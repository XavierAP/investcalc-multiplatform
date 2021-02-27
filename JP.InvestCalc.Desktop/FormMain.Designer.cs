namespace JP.InvestCalc
{
	partial class FormMain
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
			if (disposing && (components != null))
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			this.pnlTab = new System.Windows.Forms.TableLayoutPanel();
			this.txtReturnSelected = new System.Windows.Forms.TextBox();
			this.txtReturnAvg = new System.Windows.Forms.TextBox();
			this.lblReturnSelected = new System.Windows.Forms.Label();
			this.lblReturnAvg = new System.Windows.Forms.Label();
			this.table = new System.Windows.Forms.DataGridView();
			this.colStock = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colShares = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colReturn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mnuOperate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuBuy = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuSell = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuDividend = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCost = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuHistory = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuData = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuExport = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuImport = new System.Windows.Forms.ToolStripMenuItem();
			this.txtValueSelected = new System.Windows.Forms.TextBox();
			this.txtValueTotal = new System.Windows.Forms.TextBox();
			this.lblDate = new System.Windows.Forms.Label();
			this.lblValueSelected = new System.Windows.Forms.Label();
			this.lblValueTotal = new System.Windows.Forms.Label();
			this.pnl = new System.Windows.Forms.Panel();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.pnlTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
			this.mnuOperate.SuspendLayout();
			this.pnl.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(156, 6);
			// 
			// pnlTab
			// 
			this.pnlTab.ColumnCount = 4;
			this.pnlTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.pnlTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.pnlTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.pnlTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.pnlTab.Controls.Add(this.txtReturnSelected, 1, 3);
			this.pnlTab.Controls.Add(this.txtReturnAvg, 3, 3);
			this.pnlTab.Controls.Add(this.lblReturnSelected, 0, 3);
			this.pnlTab.Controls.Add(this.lblReturnAvg, 2, 3);
			this.pnlTab.Controls.Add(this.table, 0, 1);
			this.pnlTab.Controls.Add(this.txtValueSelected, 1, 2);
			this.pnlTab.Controls.Add(this.txtValueTotal, 3, 2);
			this.pnlTab.Controls.Add(this.lblDate, 0, 0);
			this.pnlTab.Controls.Add(this.lblValueSelected, 0, 2);
			this.pnlTab.Controls.Add(this.lblValueTotal, 2, 2);
			this.pnlTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlTab.Location = new System.Drawing.Point(10, 10);
			this.pnlTab.Margin = new System.Windows.Forms.Padding(4);
			this.pnlTab.Name = "pnlTab";
			this.pnlTab.RowCount = 4;
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.pnlTab.Size = new System.Drawing.Size(562, 433);
			this.pnlTab.TabIndex = 0;
			// 
			// txtReturnSelected
			// 
			this.txtReturnSelected.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtReturnSelected.Location = new System.Drawing.Point(151, 403);
			this.txtReturnSelected.Name = "txtReturnSelected";
			this.txtReturnSelected.ReadOnly = true;
			this.txtReturnSelected.Size = new System.Drawing.Size(136, 27);
			this.txtReturnSelected.TabIndex = 7;
			this.txtReturnSelected.TabStop = false;
			this.txtReturnSelected.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtReturnAvg
			// 
			this.txtReturnAvg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtReturnAvg.Location = new System.Drawing.Point(423, 403);
			this.txtReturnAvg.Name = "txtReturnAvg";
			this.txtReturnAvg.ReadOnly = true;
			this.txtReturnAvg.Size = new System.Drawing.Size(136, 27);
			this.txtReturnAvg.TabIndex = 9;
			this.txtReturnAvg.TabStop = false;
			this.txtReturnAvg.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// lblReturnSelected
			// 
			this.lblReturnSelected.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblReturnSelected.AutoSize = true;
			this.lblReturnSelected.Location = new System.Drawing.Point(3, 406);
			this.lblReturnSelected.Name = "lblReturnSelected";
			this.lblReturnSelected.Size = new System.Drawing.Size(142, 20);
			this.lblReturnSelected.TabIndex = 6;
			this.lblReturnSelected.Text = "Return of selcted:";
			// 
			// lblReturnAvg
			// 
			this.lblReturnAvg.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblReturnAvg.AutoSize = true;
			this.lblReturnAvg.Location = new System.Drawing.Point(293, 406);
			this.lblReturnAvg.Name = "lblReturnAvg";
			this.lblReturnAvg.Size = new System.Drawing.Size(124, 20);
			this.lblReturnAvg.TabIndex = 8;
			this.lblReturnAvg.Text = "Average return:";
			// 
			// table
			// 
			this.table.AllowUserToAddRows = false;
			this.table.AllowUserToDeleteRows = false;
			this.table.AllowUserToResizeRows = false;
			this.table.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.table.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.table.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStock,
            this.colShares,
            this.colPrice,
            this.colValue,
            this.colReturn});
			this.pnlTab.SetColumnSpan(this.table, 4);
			this.table.ContextMenuStrip = this.mnuOperate;
			this.table.Dock = System.Windows.Forms.DockStyle.Fill;
			this.table.Location = new System.Drawing.Point(4, 24);
			this.table.Margin = new System.Windows.Forms.Padding(4);
			this.table.Name = "table";
			this.table.RowHeadersVisible = false;
			this.table.RowHeadersWidth = 51;
			this.table.RowTemplate.Height = 24;
			this.table.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.table.Size = new System.Drawing.Size(554, 339);
			this.table.TabIndex = 1;
			// 
			// colStock
			// 
			this.colStock.HeaderText = "Stock";
			this.colStock.MinimumWidth = 6;
			this.colStock.Name = "colStock";
			this.colStock.ReadOnly = true;
			// 
			// colShares
			// 
			this.colShares.HeaderText = "Shares";
			this.colShares.MinimumWidth = 6;
			this.colShares.Name = "colShares";
			this.colShares.ReadOnly = true;
			// 
			// colPrice
			// 
			this.colPrice.HeaderText = "Price";
			this.colPrice.MinimumWidth = 6;
			this.colPrice.Name = "colPrice";
			// 
			// colValue
			// 
			this.colValue.HeaderText = "Value";
			this.colValue.MinimumWidth = 6;
			this.colValue.Name = "colValue";
			this.colValue.ReadOnly = true;
			// 
			// colReturn
			// 
			this.colReturn.HeaderText = "Yearly";
			this.colReturn.MinimumWidth = 6;
			this.colReturn.Name = "colReturn";
			this.colReturn.ReadOnly = true;
			// 
			// mnuOperate
			// 
			this.mnuOperate.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.mnuOperate.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.mnuOperate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuBuy,
            this.mnuSell,
            this.mnuDividend,
            this.mnuCost,
            toolStripSeparator1,
            this.mnuHistory,
            toolStripSeparator2,
            this.mnuData,
            this.mnuExport,
            this.mnuImport});
			this.mnuOperate.Name = "mnuOperate";
			this.mnuOperate.Size = new System.Drawing.Size(160, 240);
			// 
			// mnuBuy
			// 
			this.mnuBuy.Name = "mnuBuy";
			this.mnuBuy.Size = new System.Drawing.Size(159, 28);
			this.mnuBuy.Text = "Buy";
			// 
			// mnuSell
			// 
			this.mnuSell.Name = "mnuSell";
			this.mnuSell.Size = new System.Drawing.Size(159, 28);
			this.mnuSell.Text = "Sell";
			// 
			// mnuDividend
			// 
			this.mnuDividend.Name = "mnuDividend";
			this.mnuDividend.Size = new System.Drawing.Size(159, 28);
			this.mnuDividend.Text = "Dividend";
			// 
			// mnuCost
			// 
			this.mnuCost.Name = "mnuCost";
			this.mnuCost.Size = new System.Drawing.Size(159, 28);
			this.mnuCost.Text = "Cost";
			// 
			// mnuHistory
			// 
			this.mnuHistory.Name = "mnuHistory";
			this.mnuHistory.Size = new System.Drawing.Size(159, 28);
			this.mnuHistory.Text = "History";
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(156, 6);
			// 
			// mnuData
			// 
			this.mnuData.Name = "mnuData";
			this.mnuData.Size = new System.Drawing.Size(159, 28);
			this.mnuData.Text = "Stock data";
			// 
			// mnuExport
			// 
			this.mnuExport.Name = "mnuExport";
			this.mnuExport.Size = new System.Drawing.Size(159, 28);
			this.mnuExport.Text = "Export file";
			// 
			// mnuImport
			// 
			this.mnuImport.Name = "mnuImport";
			this.mnuImport.Size = new System.Drawing.Size(159, 28);
			this.mnuImport.Text = "Import file";
			// 
			// txtValueSelected
			// 
			this.txtValueSelected.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtValueSelected.Location = new System.Drawing.Point(151, 370);
			this.txtValueSelected.Name = "txtValueSelected";
			this.txtValueSelected.ReadOnly = true;
			this.txtValueSelected.Size = new System.Drawing.Size(136, 27);
			this.txtValueSelected.TabIndex = 3;
			this.txtValueSelected.TabStop = false;
			this.txtValueSelected.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtValueTotal
			// 
			this.txtValueTotal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtValueTotal.Location = new System.Drawing.Point(423, 370);
			this.txtValueTotal.Name = "txtValueTotal";
			this.txtValueTotal.ReadOnly = true;
			this.txtValueTotal.Size = new System.Drawing.Size(136, 27);
			this.txtValueTotal.TabIndex = 5;
			this.txtValueTotal.TabStop = false;
			this.txtValueTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// lblDate
			// 
			this.lblDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblDate.AutoSize = true;
			this.pnlTab.SetColumnSpan(this.lblDate, 4);
			this.lblDate.Location = new System.Drawing.Point(3, 0);
			this.lblDate.Name = "lblDate";
			this.lblDate.Size = new System.Drawing.Size(91, 20);
			this.lblDate.TabIndex = 0;
			this.lblDate.Text = "## Date ##";
			// 
			// lblValueSelected
			// 
			this.lblValueSelected.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblValueSelected.AutoSize = true;
			this.lblValueSelected.Location = new System.Drawing.Point(21, 373);
			this.lblValueSelected.Name = "lblValueSelected";
			this.lblValueSelected.Size = new System.Drawing.Size(124, 20);
			this.lblValueSelected.TabIndex = 2;
			this.lblValueSelected.Text = "Value selected:";
			// 
			// lblValueTotal
			// 
			this.lblValueTotal.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblValueTotal.AutoSize = true;
			this.lblValueTotal.Location = new System.Drawing.Point(305, 373);
			this.lblValueTotal.Name = "lblValueTotal";
			this.lblValueTotal.Size = new System.Drawing.Size(112, 20);
			this.lblValueTotal.TabIndex = 4;
			this.lblValueTotal.Text = "TOTAL value:";
			// 
			// pnl
			// 
			this.pnl.AutoSize = true;
			this.pnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnl.Controls.Add(this.pnlTab);
			this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnl.Location = new System.Drawing.Point(0, 0);
			this.pnl.Name = "pnl";
			this.pnl.Padding = new System.Windows.Forms.Padding(10);
			this.pnl.Size = new System.Drawing.Size(582, 453);
			this.pnl.TabIndex = 0;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(582, 453);
			this.Controls.Add(this.pnl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MinimumSize = new System.Drawing.Size(500, 400);
			this.Name = "FormMain";
			this.Text = "Return on investments";
			this.pnlTab.ResumeLayout(false);
			this.pnlTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
			this.mnuOperate.ResumeLayout(false);
			this.pnl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView table;
		private System.Windows.Forms.TextBox txtValueSelected;
		private System.Windows.Forms.TextBox txtValueTotal;
		private System.Windows.Forms.Panel pnl;
		private System.Windows.Forms.Label lblDate;
		private System.Windows.Forms.TextBox txtReturnSelected;
		private System.Windows.Forms.TextBox txtReturnAvg;
		private System.Windows.Forms.ToolStripMenuItem mnuBuy;
		private System.Windows.Forms.ToolStripMenuItem mnuSell;
		private System.Windows.Forms.ToolStripMenuItem mnuDividend;
		private System.Windows.Forms.ToolStripMenuItem mnuCost;
		private System.Windows.Forms.TableLayoutPanel pnlTab;
		private System.Windows.Forms.Label lblReturnSelected;
		private System.Windows.Forms.Label lblReturnAvg;
		private System.Windows.Forms.ContextMenuStrip mnuOperate;
		private System.Windows.Forms.Label lblValueSelected;
		private System.Windows.Forms.Label lblValueTotal;
		private System.Windows.Forms.DataGridViewTextBoxColumn colStock;
		private System.Windows.Forms.DataGridViewTextBoxColumn colShares;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
		private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
		private System.Windows.Forms.DataGridViewTextBoxColumn colReturn;
		private System.Windows.Forms.ToolStripMenuItem mnuHistory;
		private System.Windows.Forms.ToolStripMenuItem mnuData;
		private System.Windows.Forms.ToolStripMenuItem mnuExport;
		private System.Windows.Forms.ToolStripMenuItem mnuImport;
	}
}

