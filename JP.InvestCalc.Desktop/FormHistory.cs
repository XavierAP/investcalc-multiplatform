using JP.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal partial class FormHistory :Form
	{
		private readonly FlowEditor model;

		/// <summary>Keeps track of ordering for <see cref="IsDeleteAllowable"/>, in case the user reorders the table by clicking on the column headers.</summary>
		private readonly DataGridViewRow[] rowsOrdered;

		public FormHistory(FlowEditor model, IEnumerable<string> stocks, DateTime dateFrom, DateTime dateTo)
		{
			Debug.Assert(model != null && stocks != null);
			this.model = model;

			var portfolio = stocks.ToArray();
			Debug.Assert(portfolio.Length > 0);
			deleteCheckCache = new HashSet<string>(portfolio.Length);

			InitializeComponent();

			/* We use these events just to control what menu options are available
			 * or grayed out upon right-click, depending on the selection.
			 * In the chronological order they are triggered: */
			table.MouseDown += Table_MouseDown;
			table.CellMouseDown += Table_CellMouseDown;

			mnuDelete.Click += DoDelete;
			mnuExport.Click += DoExport;
			mnuImport.Click += DoImport;

			colShares.DefaultCellStyle.Alignment =
			colFlow  .DefaultCellStyle.Alignment =
			colPrice .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			colFlow.DefaultCellStyle.Format =
			colPrice.DefaultCellStyle.Format = "C2";

			var rowData = model.GetFlowDetailsOrdered(portfolio, dateFrom, dateTo).ToArray();
			rowsOrdered = new DataGridViewRow[rowData.Length];
			for(int idata = 0; idata < rowData.Length; ++idata)
			{
				var r = rowData[idata];
				var igui =
				table.Rows.Add(r.Date, r.StockName, r.Shares, r.Flow, r.PriceAvg, r.Comment);
				(rowsOrdered[idata] = table.Rows[igui])
					.Tag = r.Id;
			}
			Debug.Assert(rowsOrdered.Length == table.Rows.Count);

			headers = (
				from c in DataColumns
				select table.Columns[c].HeaderText
				).ToArray();
		}


		private IEnumerable<int> DataColumns =>
			from DataGridViewColumn col in table.Columns
			where col != colPrice // the avg price is derived data for info
			select col.Index;


		private void Table_MouseDown(object sender, MouseEventArgs ea)
		{
			// Gray these options out by default from the context menu, because they aren't valid if the user clicked on an empty area; just afterwards, CellMouseDown chooses what to enable.
			mnuDelete.Enabled =
			mnuExport.Enabled = false;
		}

		private void Table_CellMouseDown(object sender, DataGridViewCellMouseEventArgs ea)
		{
			if(ea.RowIndex < 0) return; // happens -1 if the user clicks on the headers.

			var rowClicked = table.Rows[ea.RowIndex];
			/* Right-clicking on a DataGridView does not change selection by default.
			 * We want it to select the right-clicked row, if not already selected,
			 * unless there is already another multiple selection, which may be annoying to lose. */
			if(ea.Button == MouseButtons.Right)
			{
				if(!rowClicked.Selected && table.SelectedRows.Count <= 1)
				{
					table.CurrentCell = rowClicked.Cells[ea.ColumnIndex];
					Debug.Assert(rowClicked.Selected);
				}
				mnuExport.Enabled = rowClicked.Selected;
			}
			mnuDelete.Enabled = rowClicked.Selected && IsDeleteAllowable();
			mnuDelete.ToolTipText = mnuDelete.Enabled ? null : "Only the chronologically last operation(s) on each stock may be deleted.";
		}
		

		private bool IsDeleteAllowable()
		{
			deleteCheckCache.Clear();
			foreach(var row in rowsOrdered.Reverse())
			{
				if(row.Selected)
				{
					// allow deletion only of the last flow(s) for each stock:
					if(deleteCheckCache.Contains(GetStockName(row)))
						return false;
				}
				else // note down what stocks have been operated chronologically after the records to delete:
					deleteCheckCache.Add(GetStockName(row));
			}
			return true;
		}

		private readonly HashSet<string> deleteCheckCache;

		private string GetStockName(DataGridViewRow row)
			=> (string)row.Cells[colStock.Index].Value;

		private void DoDelete(object sender, EventArgs ea)
		{
			Debug.Assert(table.SelectedRows.Count > 0);
			
			// Compose confirmation message:
			var msg = table.SelectedRows.Count > 1 ?
				"Are you sure you want to delete ALL the selected records?" :
				"Are you sure you want to delete the selected record?" ;

			var ans = MessageBox.Show(this, msg, Config.AppName,
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

			if(ans != DialogResult.Yes) return;

			// Delete from database:
			Debug.Assert(IsDeleteAllowable());
			model.DeleteFlows(
				from DataGridViewRow row in table.SelectedRows
				select (long)row.Tag );

			// Delete from GUI:
			table.SuspendLayout();

			foreach(DataGridViewRow row in table.SelectedRows)
				table.Rows.Remove(row);

			table.ResumeLayout();
		}


		private readonly StringBuilder csv = new StringBuilder();
		private readonly string csvSeparator = GetCSVSeparator();

		private void DoExport(object sender, EventArgs ea)
		{
			csv.Clear();

			foreach(DataGridViewRow row in table.Rows) // iterate all .Rows to preserve display order; .SelectedRows may have a different order
			{
				if(!row.Selected) continue;

				foreach(var c in DataColumns)
					csv.Append(row.Cells[c].Value).Append(csvSeparator);

				BackDown(csv, csvSeparator)
					.AppendLine();
			}

			using(var dlg = new FormTextPad(true, headers, csv.ToString()))
				dlg.ShowDialog(this);
		}

		private void DoImport(object sender, EventArgs ea)
		{
			using(var dlg = new FormTextPad(false, headers, null))
			{
				int n;
				do {
					dlg.ShowDialog(this);
					if(dlg.DialogResult == DialogResult.Cancel)
						return;

					try { n = model.ImportFlows(dlg.Content, csvSeparator); }
					catch(DataException err)
					{
						err.Display();
						n = -1;
					}
				}
				while(n <= 0);
				Close(); // the displayed data are no longer up to date after importing
			}
		}

		private static string GetCSVSeparator()
		{
			string sep = Properties.Settings.Default.csvSeparator;
			if(string.IsNullOrEmpty(sep)) sep = "\t";
			return sep;
		}


		private static StringBuilder
		BackDown(StringBuilder text, string trail)
		{
			Debug.Assert( text[text.Length - trail.Length] == trail[0] );
			return text.Remove(text.Length - trail.Length, trail.Length);
		}

		private readonly string[] headers;
	}
}
