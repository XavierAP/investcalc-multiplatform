using JP.SQLite;
using System;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal partial class FormStockData : Form
	{
		public FormStockData(object dataSource,
			Action onSaveData,
			Action<string> onSaveApiLicense)
		{
			this.onSaveApiLicense = onSaveApiLicense;

			InitializeComponent();
			InitializeGridView(dataSource, onSaveData);
			InitializeLicenseInput();

			btnSearch.Click += (s,e) =>
			{
				using(var dlg = new FormSearch(txtLicense.Text))
					dlg.ShowDialog();
			};
		}

		private void InitializeGridView(object dataSource, Action onSaveData)
		{
			var table = new SQLiteGridView(dataSource, 1)
			{
				Dock = DockStyle.Fill,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
				RowHeadersVisible = false,
			};
			layoutPanel.Controls.Add(table, 0, 1);
			layoutPanel.SetColumnSpan(table, 2);

			btnSave.Click += (s, ea) =>
			{
				try
				{
					onSaveData();
					Close();
				}
				catch(Exception err)
				{
					err.Display();
					DialogResult = DialogResult.Retry;
				}
			};

			Disposed += (s,e) => table.Dispose();
		}

		#region License key input

		private void InitializeLicenseInput()
		{
			txtLicense = new TextBox
			{
				Text = Properties.Settings.Default.ApiLicense,
				Dock = DockStyle.Fill,
				TabStop = btnLicense.TabStop,
				TabIndex = btnLicense.TabIndex,
			};

			btnLicense.Click     += StartLicenseInput;
			txtLicense.KeyDown   += OnKeyDownWhileLicenseInput;
			txtLicense.LostFocus += ConfirmLicenseInput;

			Disposed += (s,e) => txtLicense.Dispose();

			OnLicenseChanged();
		}

		private void StartLicenseInput(object sender, EventArgs ea)
		{
			if(IsFirstTimeLicenseInput)
			{
				IsFirstTimeLicenseInput = false;
				if(string.IsNullOrWhiteSpace(txtLicense.Text)) MessageBox.Show(
					"To get a free license go to https://www.alphavantage.co (not affiliated).",
					Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			txtLicenseBackup = txtLicense.Text;
			SwapControlsInTableLayoutPanel(layoutPanel, btnLicense, txtLicense);
			txtLicense.SelectAll();
			txtLicense.Focus();
		}
		
		private void OnKeyDownWhileLicenseInput(object sender, KeyEventArgs ea)
		{
			if(ea.KeyCode == Keys.Enter)
				ConfirmLicenseInput(default, default);
			else if(ea.KeyCode == Keys.Escape)
				CancelLicenseInput();
		}

		private void ConfirmLicenseInput(object sender, EventArgs ea)
		{
			OnLicenseChanged();
			onSaveApiLicense(txtLicense.Text);
			SwapControlsInTableLayoutPanel(layoutPanel, txtLicense, btnLicense);
		}

		private void OnLicenseChanged()
		{
			btnSearch.Enabled = !string.IsNullOrEmpty(txtLicense.Text);
		}

		private void CancelLicenseInput()
		{
			SwapControlsInTableLayoutPanel(layoutPanel, txtLicense, btnLicense);
			txtLicense.Text = txtLicenseBackup;
		}

		TextBox txtLicense;
		string txtLicenseBackup;
		static bool IsFirstTimeLicenseInput = true;

		readonly Action<string> onSaveApiLicense;

		private static void SwapControlsInTableLayoutPanel(TableLayoutPanel parent,
			Control current, Control substitute)
		{
			var position = parent.GetCellPosition(current);
			parent.SuspendLayout();
			parent.Controls.Remove(current);
			parent.Controls.Add(substitute, position.Column, position.Row);
			parent.ResumeLayout();
		}

		#endregion
	}
}
