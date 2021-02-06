using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal partial class FormMain : Form, IPortfolioView
	{
		private readonly ModelGateway model;

		internal FormMain(ModelGateway model)
		{
			this.model = model;

			InitializeComponent();
			table.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			colStock.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			colReturn.DefaultCellStyle.Format = "P" + Config.PrecisionPerCent;

			UpdateTime();
			FillTable();

			mnuBuy     .Click += (s, e) => OpRecord(Operation.Buy);
			mnuSell    .Click += (s, e) => OpRecord(Operation.Sell);
			mnuDividend.Click += (s, e) => OpRecord(Operation.Dividend);
			mnuCost    .Click += (s, e) => OpRecord(Operation.Cost);

			mnuHistory.Click += OpsHistory;

			table.CellValidating += ValidatingInput;
			table.SelectionChanged += SelectionChanged;
			SelectionChanged(null, null);
		}


		public void AddStock(string name, double shares, double? returnPer1Yearly)
		{
			table.Rows.Add(name, shares, null, null,
				returnPer1Yearly.HasValue ? (object)returnPer1Yearly.Value : null);

			txtReturnAvg.Text = null;
		}

		public void SetStockFigures(Stock stk, double? returnPer1Yearly)
		{
			var irow = GetRow(stk.Name);
			GetCell(irow, colShares).Value = stk.Shares;
			if(stk.Price.HasValue)
			{
				GetCell(irow, colPrice).Value = stk.Price.Value;
				GetCell(irow, colValue).Value = Math.Round(stk.Price.Value * stk.Shares, 2);
			}
			else
			{
				GetCell(irow, colPrice).Value =
				GetCell(irow, colValue).Value = null;
			}
			if(returnPer1Yearly.HasValue)
				GetCell(irow, colReturn).Value = returnPer1Yearly.Value;
			else
				GetCell(irow, colReturn).Value = null;

			TryCalcReturnAvg(table.Rows);
		}

		public void InvokeOnUIThread(Action action) => Invoke(action);


		private void FillTable()
		{
			table.SuspendLayout();
			table.Rows.Clear();
			model.Portfolio.Load(this);
			TryCalcReturnAvg(table.Rows);
			table.ResumeLayout();
			
			if(IsStartupWithEmptyPortfolio) Shown += PromptHelpOnStartup;
		}

		private bool IsStartupWithEmptyPortfolio => !Visible && table.Rows.Count == 0;

		private void PromptHelpOnStartup(object sender, EventArgs ea)
		{
			MessageBox.Show(this, "Right-click on the table for options and commands.", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			Shown -= PromptHelpOnStartup;
		}

		private void UpdateTime() => lblDate.Text = DateTime.Now.Date.ToLongDateString();


		private void ValidatingInput(object sender, DataGridViewCellValidatingEventArgs ea)
		{
			if(ea.ColumnIndex != colPrice.Index) return;

			var priceInput = (string)ea.FormattedValue;
			var priceCell = GetCell(ea.RowIndex, colPrice);

			if(string.IsNullOrEmpty(priceInput))
			{
				ea.Cancel = priceCell.Value != null;
			}
			else if(double.TryParse(priceInput, out var price) &&
				ValidatePrice(price))
			{
				table[ea.ColumnIndex, ea.RowIndex].ToolTipText = null; // clear possible error messages from previous input
				priceCell.Value = price;
				var shares = (double)GetCell(ea.RowIndex, colShares).Value;
				GetCell(ea.RowIndex, colValue).Value = Math.Round(price * shares, 2);
				var stockName = (string)GetCell(ea.RowIndex, colStock).Value;
				GetCell(ea.RowIndex, colReturn).Value = model.Calculator.CalcReturn(stockName, shares, price);
			}
			else ea.Cancel = true;
		}

		private bool ValidatePrice(double price)
		{
			if(price < 0)
			{
				MessageBox.Show(this, "Prices must be positive, real numbers.", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			else return true;
		}

		private void TryCalcReturnAvg(IList dataGridViewRowsToAverage)
		{
			UpdateTime();
			bool selectedStocksOnly = dataGridViewRowsToAverage != table.Rows;
			TextBox
				txtValue  = selectedStocksOnly ? txtValueSelected : txtValueTotal,
				txtReturn = selectedStocksOnly ? txtReturnSelected : txtReturnAvg;

			double total = 0;
			var stocks = new string[dataGridViewRowsToAverage.Count];
			int i = 0;
			foreach(DataGridViewRow row in dataGridViewRowsToAverage)
			{
				object content = GetCell(row, colValue).Value;
				if(content == null)
				{
					if((double)GetCell(row, colShares).Value != 0)
					{
						txtValue.Text = txtReturn.Text = null;
						return;
					}
				}
				else total += (double)content;

				stocks[i++] = (string)GetCell(row, colStock).Value;
			}

			txtValue.Text = total.ToString("C2");
			txtReturn.Text = model.Calculator.CalcReturnAvg(stocks, total)
				.ToString(colReturn.DefaultCellStyle.Format);
		}


		private void SelectionChanged(object sender, EventArgs ea)
		{
			bool multi = table.SelectedRows.Count > 1;

			txtValueSelected.Visible =
			lblValueSelected.Visible =
			txtReturnSelected.Visible =
			lblReturnSelected.Visible = multi;

			if(multi) TryCalcReturnAvg(table.SelectedRows);
		}


		private void OpRecord(Operation op)
		{
			using(var dlg = new FormOp(op, GetPortfolio()))
			{
				var ans = dlg.ShowDialog(this);
				if(ans != DialogResult.OK) return;
				model.Portfolio.AddShares(this, dlg.StockName, dlg.Shares, dlg.Total, dlg.Date, dlg.Comment);
			}
		}


		/// <summary>Lets the user browse past operations.</summary>
		private void OpsHistory(object sender, EventArgs ea)
		{
			int n = table.Rows.Count;
			var selected = new bool[n];
			var stockNames = new string[n];
			int multi = 0;
			int i = 0;
			foreach(DataGridViewRow row in table.Rows)
			{
				stockNames[i] = (string)GetCell(i, colStock).Value;

				if(selected[i] = row.Selected)
					++multi;

				++i;
			}

			var dataSource = model.Flows.GetFlowEditor();
			using(var dlg = new FormHistoryFilter(dataSource, stockNames, multi>1?selected:null))
				dlg.ShowDialog(this);

			if(dataSource.IsDataChanged) FillTable();
		}


		private Dictionary<string, double>
		GetPortfolio() => (
			from DataGridViewRow row in table.Rows
			let name   = (string)GetCell(row.Index, colStock ).Value
			let shares = (double)GetCell(row.Index, colShares).Value
			select (name, shares)
			).ToDictionary(tuple => tuple.name, tuple => tuple.shares);

		private DataGridViewCell
		GetCell(int irow, DataGridViewColumn col)
			=> GetCell(table.Rows[irow], col);
		
		private static DataGridViewCell
		GetCell(DataGridViewRow row, DataGridViewColumn col)
			=> row.Cells[col.Index];

		private int
		GetRow(string stockName) => (
			from DataGridViewRow row in table.Rows
			where stockName == (string)GetCell(row, colStock).Value
			select row.Index
			).Single();
	}
}
