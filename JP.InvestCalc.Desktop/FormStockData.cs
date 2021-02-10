using JP.SQLite;
using System;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal partial class FormStockData : Form
	{
		public FormStockData(object dataSource, Action onSaveUpdateDataSource)
		{
			InitializeComponent();
			InitializeGridView(dataSource, onSaveUpdateDataSource);
			InitializeLicenseInput();
			OnLicenseChanged();
		}

		private SQLiteGridView InitializeGridView(object dataSource, Action onSaveUpdateDataSource)
		{
			var Table = new SQLiteGridView(dataSource, 1)
			{
				AutoSize = true,
				Dock = DockStyle.Fill,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
			};
			layoutPanel.Controls.Add(Table, 0, 1);
			layoutPanel.SetColumnSpan(Table, 2);

			btnSave.Click += (s, ea) =>
			{
				try
				{
					onSaveUpdateDataSource();
					Close();
				}
				catch(Exception err)
				{
					err.Display();
					DialogResult = DialogResult.Retry;
				}
			};

			return Table;
		}

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

			btnSearch.Click += (s, e) => MessageBox.Show("Not implemented yet :(");
		}

		private void StartLicenseInput(object sender, EventArgs ea)
		{
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
			Properties.Settings.Default.ApiLicense = txtLicense.Text;
			Properties.Settings.Default.Save();
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

		private static void SwapControlsInTableLayoutPanel(TableLayoutPanel parent,
			Control current, Control substitute)
		{
			var position = parent.GetCellPosition(current);
			parent.SuspendLayout();
			parent.Controls.Remove(current);
			parent.Controls.Add(substitute, position.Column, position.Row);
			parent.ResumeLayout();
		}
	}
}
