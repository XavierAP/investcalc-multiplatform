using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal partial class FormSearch : Form
	{
		public FormSearch(string apiLicenseKey)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(apiLicenseKey));
			this.LicenseKey = apiLicenseKey;

			InitializeComponent();

			KeyPreview = true;
			KeyDown += OnKeyDownAnywhere;
			txtSearch.KeyDown += (s,ea) => { if(ea.KeyCode == Keys.Enter) Search(); };
			btnSearch.Click   += (s,ea) => Search();

			table.CellDoubleClick += OnDoubleClickInTable;
		}

		readonly string LicenseKey;
		readonly StockSearcher searcher = new StockSearcher();

		private async void Search()
		{
			if(string.IsNullOrWhiteSpace(txtSearch.Text) ||
				!btnSearch.Enabled)
			{
				txtSearch.Focus();
				return;
			}

			btnSearch.Enabled = false;
			string[][] results = null;
			try
			{
				results = await searcher.Search(txtSearch.Text, LicenseKey);
			}
			catch(Exception err)
			{
				err.Display();
			}
			if(!IsDisposed) Invoke(new Action( () =>
			{
				if(results != null)
				{
					table.Rows.Clear();
					for(int r = 0; r < results.Length; r++)
						table.Rows.Add(results[r]);
				}
				btnSearch.Enabled = true;
				txtSearch.Focus();
			} ));
		}

		private void OnDoubleClickInTable(object sender, DataGridViewCellEventArgs ea)
		{
			if(ea.RowIndex < 0) return; // clicked on header

			var value = (string)table.Rows[ea.RowIndex].Cells[colCode.Index].Value;
			Clipboard.SetText(value);
			Close();
			MessageBox.Show($"{colCode.HeaderText} '{value}' copied to clipboard.",
				Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void OnKeyDownAnywhere(object sender, KeyEventArgs ea)
		{
			if(ea.KeyCode == Keys.Escape)
			{
				DialogResult = DialogResult.Cancel;
				Close();
			}
		}
	}
}
