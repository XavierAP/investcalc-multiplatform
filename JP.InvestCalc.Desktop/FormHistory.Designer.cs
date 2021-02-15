namespace JP.InvestCalc
{
	partial class FormHistory
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
			this.components = new System.ComponentModel.Container();
			this.table = new System.Windows.Forms.DataGridView();
			this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colStock = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colShares = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colFlow = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mnuCommands = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuExport = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuImport = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
			this.mnuCommands.SuspendLayout();
			this.SuspendLayout();
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
            this.colDate,
            this.colStock,
            this.colShares,
            this.colFlow,
            this.colPrice,
            this.colComment});
			this.table.ContextMenuStrip = this.mnuCommands;
			this.table.Dock = System.Windows.Forms.DockStyle.Fill;
			this.table.Location = new System.Drawing.Point(0, 0);
			this.table.Margin = new System.Windows.Forms.Padding(4);
			this.table.Name = "table";
			this.table.RowHeadersVisible = false;
			this.table.RowTemplate.Height = 24;
			this.table.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.table.Size = new System.Drawing.Size(782, 553);
			this.table.TabIndex = 2;
			// 
			// colDate
			// 
			this.colDate.HeaderText = "Date";
			this.colDate.Name = "colDate";
			this.colDate.ReadOnly = true;
			// 
			// colStock
			// 
			this.colStock.HeaderText = "Stock";
			this.colStock.Name = "colStock";
			this.colStock.ReadOnly = true;
			// 
			// colShares
			// 
			this.colShares.HeaderText = "Shares";
			this.colShares.Name = "colShares";
			this.colShares.ReadOnly = true;
			// 
			// colFlow
			// 
			this.colFlow.HeaderText = "Flow";
			this.colFlow.Name = "colFlow";
			this.colFlow.ReadOnly = true;
			// 
			// colPrice
			// 
			this.colPrice.HeaderText = "Price (info)";
			this.colPrice.Name = "colPrice";
			this.colPrice.ReadOnly = true;
			// 
			// colComment
			// 
			this.colComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.colComment.HeaderText = "Comment";
			this.colComment.Name = "colComment";
			this.colComment.ReadOnly = true;
			this.colComment.Width = 159;
			// 
			// mnuCommands
			// 
			this.mnuCommands.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.mnuCommands.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.mnuCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDelete,
            this.mnuExport,
            this.mnuImport});
			this.mnuCommands.Name = "mnu";
			this.mnuCommands.Size = new System.Drawing.Size(214, 116);
			// 
			// mnuDelete
			// 
			this.mnuDelete.Name = "mnuDelete";
			this.mnuDelete.Size = new System.Drawing.Size(213, 28);
			this.mnuDelete.Text = "Delete";
			// 
			// mnuExport
			// 
			this.mnuExport.Name = "mnuExport";
			this.mnuExport.Size = new System.Drawing.Size(213, 28);
			this.mnuExport.Text = "Export to CSV";
			// 
			// mnuImport
			// 
			this.mnuImport.Name = "mnuImport";
			this.mnuImport.Size = new System.Drawing.Size(213, 28);
			this.mnuImport.Text = "Import from CSV";
			// 
			// FormHistory
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(782, 553);
			this.Controls.Add(this.table);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MinimizeBox = false;
			this.Name = "FormHistory";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Operation history";
			((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
			this.mnuCommands.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView table;
		private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn colStock;
		private System.Windows.Forms.DataGridViewTextBoxColumn colShares;
		private System.Windows.Forms.DataGridViewTextBoxColumn colFlow;
		private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
		private System.Windows.Forms.DataGridViewTextBoxColumn colComment;
		private System.Windows.Forms.ContextMenuStrip mnuCommands;
		private System.Windows.Forms.ToolStripMenuItem mnuDelete;
		private System.Windows.Forms.ToolStripMenuItem mnuExport;
		private System.Windows.Forms.ToolStripMenuItem mnuImport;
	}
}
