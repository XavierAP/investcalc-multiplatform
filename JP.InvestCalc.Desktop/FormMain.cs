﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal partial class FormMain : Form, PortfolioView
	{
		private readonly ModelGateway Model;

		internal FormMain(ModelGateway model)
		{
			Model = model;

			InitializeComponent();
			table.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			colStock.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			colValue.DefaultCellStyle.Format =
			colNetGain.DefaultCellStyle.Format = "N2";
			colGainRatio.DefaultCellStyle.Format =
			colReturn.DefaultCellStyle.Format = "P2";

			UpdateTime();
			FillTable();

			mnuBuy     .Click += (s, e) => OpRecord(Operation.Buy);
			mnuSell    .Click += (s, e) => OpRecord(Operation.Sell);
			mnuDividend.Click += (s, e) => OpRecord(Operation.Dividend);
			mnuCost    .Click += (s, e) => OpRecord(Operation.Cost);

			mnuHistory.Click += OpsHistory;
			mnuData.Click += EditStockData;
			mnuExport.Click += ExportFile;
			mnuImport.Click += ImportFile;

			table.CellValidating += ValidatingInput;
			table.SelectionChanged += SelectionChanged;
			SelectionChanged(null, null);
		}

		void PortfolioView.AddStock(string name, double shares,
			(double NetGain, double GainRatio, double YearlyPer1)? info)
		{
			if(info.HasValue)
				table.Rows.Add(name, shares, null, null, info.Value.NetGain, info.Value.GainRatio, info.Value.YearlyPer1);
			else
				table.Rows.Add(name, shares, null, null, null, null, null);

			txtReturnAvg.Text = null;
		}

		void PortfolioView.SetStockFigures(Stock stk,
			(double NetGain, double GainRatio, double YearlyPer1)? info)
		{
			var irow = GetRow(stk.Name);
			GetCell(irow, colShares).Value = stk.Shares;
			if(stk.Price.HasValue)
			{
				GetCell(irow, colPrice).Value = stk.Price.Value;
				GetCell(irow, colValue).Value = stk.Price.Value * stk.Shares;
			}
			else
			{
				GetCell(irow, colPrice).Value =
				GetCell(irow, colValue).Value = null;
			}
			if(info.HasValue)
			{
				GetCell(irow, colNetGain).Value = info.Value.NetGain;
				GetCell(irow, colGainRatio).Value = info.Value.GainRatio;
				GetCell(irow, colReturn).Value = info.Value.YearlyPer1;
			}
			else
			{
				GetCell(irow, colNetGain).Value =
				GetCell(irow, colGainRatio).Value =
				GetCell(irow, colReturn).Value = null;
			}
			TryCalcReturnAvg(table.Rows);
		}

		void PortfolioView.InvokeOnUIThread(Action action) => Invoke(action);


		private void FillTable()
		{
			table.SuspendLayout();
			table.Rows.Clear();
			Model.Portfolio.Load(this);
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
			int irow = ea.RowIndex;
			int icol = ea.ColumnIndex;

			if(icol != colPrice.Index) return;

			var priceInput = (string)ea.FormattedValue;
			var priceCell = GetCell(irow, colPrice);

			if(string.IsNullOrEmpty(priceInput))
			{
				ea.Cancel = priceCell.Value != null;
			}
			else if(double.TryParse(priceInput, out var price) &&
				ValidatePrice(price))
			{
				table[icol, irow].ToolTipText = null; // clear possible error messages from previous input
				priceCell.Value = price;
				var shares = (double)GetCell(irow, colShares).Value;
				var total = shares * price;
				GetCell(irow, colValue).Value = total;
				var stockName = (string)GetCell(irow, colStock).Value;

				var info = Model.Calculator.CalcReturn(stockName, total);
				GetCell(irow, colNetGain).Value = info.NetGain;
				GetCell(irow, colGainRatio).Value = info.GainRatio;
				GetCell(irow, colReturn).Value = info.YearlyPer1;
			}
			else ea.Cancel = true;

			TryCalcReturnAvg(table.Rows);
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
			for(int i = 0; i < dataGridViewRowsToAverage.Count; i++)
			{
				var row = (DataGridViewRow)dataGridViewRowsToAverage[i];
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

				stocks[i] = (string)GetCell(row, colStock).Value;
			}

			txtValue.Text = total.ToString("C2");
			txtReturn.Text = Model.Calculator.CalcReturnAvg(stocks, total)
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
			using var dlg = new FormOp(op, Model.Portfolio, this);
			var ans = dlg.ShowDialog(this);
		}


		/// <summary>Lets the user browse past operations.</summary>
		private void OpsHistory(object sender, EventArgs ea)
		{
			int n = table.Rows.Count;
			var selected = new bool[n];
			var stockNames = new string[n];
			int multi = 0;
			for(int i = 0; i < table.Rows.Count; i++)
			{
				stockNames[i] = (string)GetCell(i, colStock).Value;

				if(selected[i] = table.Rows[i].Selected)
					++multi;
			}

			var dataSource = Model.Data.GetFlowEditor();
			using(var dlg = new FormHistoryFilter(dataSource, stockNames, multi>1?selected:null))
				dlg.ShowDialog(this);

			if(dataSource.IsDataChanged) FillTable();
		}

		
		private void EditStockData(object sender, EventArgs ea)
		{
			using var bindings = new DataBindings(Model);
			var data = bindings.StockBinding;
			using var dlg = new FormStockData(data,
				() => bindings.Update(data),
				OnSaveApiLicense);

			bool newLicense = false;

			retry:
			var ans = dlg.ShowDialog(this);
			
			if(ans == DialogResult.Retry)
				goto retry;
			else if(ans == DialogResult.OK || newLicense)
				FillTable();

			void OnSaveApiLicense(string licenseKey)
			{
				newLicense = !string.IsNullOrWhiteSpace(licenseKey);
				Model.ApiLicenseKey = licenseKey;
				Properties.Settings.Default.ApiLicense = licenseKey;
				Properties.Settings.Default.Save();
			}
		}

		
		private void ExportFile(object sender, EventArgs ea)
		{
			using var dlg = new SaveFileDialog
			{
				FileName = Path.GetFileName(Model.Data.FilePath),
			};
			SetInitialDirectory(dlg);
			dlg.TryIfOk(pathName => File.Copy(Model.Data.FilePath, pathName, true));
		}

		private void ImportFile(object sender, EventArgs ea)
		{
			using var dlg = new OpenFileDialog();
			SetInitialDirectory(dlg);
			dlg.TryIfOk(pathName =>
			{
				File.Copy(pathName, Model.Data.FilePath, true);
				FillTable();
			});
		}

		private static void SetInitialDirectory(FileDialog dlg) =>
			dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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
