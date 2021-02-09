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
			
			Table = new SQLiteGridView(dataSource, 1)
			{
				Parent = this,
				AutoSize = true,
				Dock = DockStyle.Fill,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
			};
			Table.BringToFront();
			
			Table.DataBindingComplete += (sender, eventArgs) =>
			{
				Table.NumberOfHiddenColumns = 1;
			};

			Table.DataSource = dataSource;

			ButtonSave.Click += (s, ea) =>
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
		}

		readonly SQLiteGridView Table;
	}
}
