
namespace JP.InvestCalc
{
	partial class FormSearch
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.TableLayoutPanel layoutPanel;
			this.table = new System.Windows.Forms.DataGridView();
			this.colCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCurrency = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colRegion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.btnSearch = new System.Windows.Forms.Button();
			layoutPanel = new System.Windows.Forms.TableLayoutPanel();
			layoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
			this.SuspendLayout();
			// 
			// layoutPanel
			// 
			layoutPanel.AutoSize = true;
			layoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			layoutPanel.ColumnCount = 2;
			layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			layoutPanel.Controls.Add(this.table, 0, 1);
			layoutPanel.Controls.Add(this.txtSearch, 0, 0);
			layoutPanel.Controls.Add(this.btnSearch, 1, 0);
			layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			layoutPanel.Location = new System.Drawing.Point(0, 0);
			layoutPanel.Name = "layoutPanel";
			layoutPanel.RowCount = 2;
			layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			layoutPanel.Size = new System.Drawing.Size(782, 253);
			layoutPanel.TabIndex = 0;
			// 
			// table
			// 
			this.table.AllowUserToAddRows = false;
			this.table.AllowUserToDeleteRows = false;
			this.table.AllowUserToResizeRows = false;
			this.table.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.table.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCode,
            this.colName,
            this.colCurrency,
            this.colRegion,
            this.colType});
			layoutPanel.SetColumnSpan(this.table, 2);
			this.table.Dock = System.Windows.Forms.DockStyle.Fill;
			this.table.Location = new System.Drawing.Point(3, 39);
			this.table.MultiSelect = false;
			this.table.Name = "table";
			this.table.ReadOnly = true;
			this.table.RowHeadersVisible = false;
			this.table.RowHeadersWidth = 51;
			this.table.RowTemplate.Height = 29;
			this.table.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.table.Size = new System.Drawing.Size(776, 211);
			this.table.TabIndex = 1;
			// 
			// colCode
			// 
			this.colCode.Frozen = true;
			this.colCode.HeaderText = "Code";
			this.colCode.MinimumWidth = 6;
			this.colCode.Name = "colCode";
			this.colCode.ReadOnly = true;
			this.colCode.Width = 73;
			// 
			// colName
			// 
			this.colName.HeaderText = "Name";
			this.colName.MinimumWidth = 6;
			this.colName.Name = "colName";
			this.colName.ReadOnly = true;
			this.colName.Width = 78;
			// 
			// colCurrency
			// 
			this.colCurrency.HeaderText = "Currency";
			this.colCurrency.MinimumWidth = 6;
			this.colCurrency.Name = "colCurrency";
			this.colCurrency.ReadOnly = true;
			this.colCurrency.Width = 95;
			// 
			// colRegion
			// 
			this.colRegion.HeaderText = "Region";
			this.colRegion.MinimumWidth = 6;
			this.colRegion.Name = "colRegion";
			this.colRegion.ReadOnly = true;
			this.colRegion.Width = 85;
			// 
			// colType
			// 
			this.colType.HeaderText = "Type";
			this.colType.MinimumWidth = 6;
			this.colType.Name = "colType";
			this.colType.ReadOnly = true;
			this.colType.Width = 69;
			// 
			// txtSearch
			// 
			this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSearch.Location = new System.Drawing.Point(3, 4);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(707, 27);
			this.txtSearch.TabIndex = 0;
			// 
			// btnSearch
			// 
			this.btnSearch.AutoSize = true;
			this.btnSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnSearch.Location = new System.Drawing.Point(716, 3);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(63, 30);
			this.btnSearch.TabIndex = 1;
			this.btnSearch.Text = "&Search";
			this.btnSearch.UseVisualStyleBackColor = true;
			// 
			// FormSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(782, 253);
			this.Controls.Add(layoutPanel);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(500, 300);
			this.Name = "FormSearch";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Search for stocks";
			layoutPanel.ResumeLayout(false);
			layoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView table;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
		private System.Windows.Forms.DataGridViewTextBoxColumn colName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCurrency;
		private System.Windows.Forms.DataGridViewTextBoxColumn colRegion;
		private System.Windows.Forms.DataGridViewTextBoxColumn colType;
	}
}