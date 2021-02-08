using System;
using System.Drawing;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal partial class FormStockData : Form
	{
		public bool AllowUserToAddRows
		{
			set => table.AllowUserToAddRows = value;
		}
		public bool AllowUserToDeleteRows
		{
			set => table.AllowUserToDeleteRows = value;
		}
		/// <summary>First N columns won't be visible to the user.</summary>
		public byte NumberOfHiddenColumns
		{
			set
			{
				for(byte i = 0; i < value; ++i)
					table.Columns[i].Visible = false;
			}
		}
		/// <summary>First N columns will stay in place when scrolling horizontally.</summary>
		public byte NumberOfFrozenColumns
		{
			set
			{
				for(byte i = 0; i < value; ++i)
					table.Columns[i].Frozen = false;
			}
		}

		public FormStockData(object dataSource, Action onSaveUpdateDataSource)
		{
			InitializeComponent();
			
			table = new DataGridView();
			table.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
			table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			table.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			table.AutoSize = true;
			table.MaximumSize = new Size(1920, 800);
			table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			table.Parent = this;
			table.BringToFront();
			table.Dock = DockStyle.Fill;
			
			table.DataBindingComplete += (sender, eventArgs) =>
				table.DataError += (s, ea) => ea.Cancel = true;

			table.DataSource = dataSource;

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

		readonly DataGridView table;
	}
}
