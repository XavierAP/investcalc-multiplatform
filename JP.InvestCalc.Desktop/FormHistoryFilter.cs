using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	/// <summary>Dialog for the user to select for what stocks
	/// and in what times they want to browse past operations.</summary>
	internal partial class FormHistoryFilter :Form
	{
		private readonly FlowEditor model;

		public FormHistoryFilter(FlowEditor model,
			IEnumerable<string> portfolio,
			IEnumerable<bool> selected )
		{
			Debug.Assert(model != null);
			this.model = model;

			InitializeComponent();
			
			listStocks.Items.AddRange(portfolio.ToArray());
			if(selected == null) SelectAll();
			else
			{
				int i = 0;
				foreach(bool select in selected.Take(listStocks.Items.Count))
					listStocks.SetSelected(i++, select);
			}

			pickDateFrom.Value = pickDateFrom.MinDate;

			KeyPreview = true;
			KeyDown += Form_KeyDown;
			Shown += (e,a) => btnOK.Focus(); // listStocks interferes with keyboard handling when it's in focus

			// Arbitrary "from", but make both same kind (local):
			pickDateFrom.Value = new DateTime(2000, 1, 1, 0,0,0, DateTimeKind.Local);
			pickDateTo  .Value = DateTime.Now.Date;

			RestrictDate(pickDateFrom);
			RestrictDate(pickDateTo  );
			pickDateFrom.ValueChanged += (s,e)=> RestrictDate(pickDateFrom);
			pickDateTo  .ValueChanged += (s,e)=> RestrictDate(pickDateTo  );

			btnOK.Click += Launch;
		}

		private void RestrictDate(DateTimePicker pickControl)
		{
			if(pickControl == pickDateFrom)
				pickDateTo.MinDate = pickDateFrom.Value;
			else if(pickControl == pickDateTo)
				pickDateFrom.MaxDate = pickDateTo.Value;
			else Debug.Assert(false);
		}

		private void SelectAll()
		{
			for(int i = 0; i < listStocks.Items.Count; ++i)
				listStocks.SetSelected(i, true);
		}

		private void Form_KeyDown(object sender, KeyEventArgs ea)
		{
			const Keys keySelectAll = Keys.Control | Keys.A;

			if(ea.KeyData == keySelectAll)
				SelectAll();
		}

		private void Launch(object sender, EventArgs ea)
		{
			if(listStocks.SelectedItems.Count <= 0)
			{
				MessageBox.Show(this, "No stocks selected.\nPress Ctrl+A to select all.", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				SelectAll();
				return;
			}

			var stocks = (
				from object item in listStocks.SelectedItems
				select (string)item
				);

			using(var dlg = new FormHistory(model, stocks, pickDateFrom.Value, pickDateTo.Value))
				dlg.ShowDialog(this);
		}
	}
}
