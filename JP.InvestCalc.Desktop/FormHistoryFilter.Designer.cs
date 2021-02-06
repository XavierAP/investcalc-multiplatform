namespace JP.InvestCalc
{
	partial class FormHistoryFilter
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
			System.Windows.Forms.Label lblDateFrom;
			System.Windows.Forms.Label lblDateTo;
			System.Windows.Forms.TableLayoutPanel pnl;
			this.listStocks = new System.Windows.Forms.ListBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pickDateFrom = new System.Windows.Forms.DateTimePicker();
			this.pickDateTo = new System.Windows.Forms.DateTimePicker();
			lblDateFrom = new System.Windows.Forms.Label();
			lblDateTo = new System.Windows.Forms.Label();
			pnl = new System.Windows.Forms.TableLayoutPanel();
			pnl.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblDateFrom
			// 
			lblDateFrom.Anchor = System.Windows.Forms.AnchorStyles.Right;
			lblDateFrom.AutoSize = true;
			lblDateFrom.Location = new System.Drawing.Point(77, 196);
			lblDateFrom.Name = "lblDateFrom";
			lblDateFrom.Size = new System.Drawing.Size(53, 20);
			lblDateFrom.TabIndex = 1;
			lblDateFrom.Text = "From:";
			// 
			// lblDateTo
			// 
			lblDateTo.Anchor = System.Windows.Forms.AnchorStyles.Right;
			lblDateTo.AutoSize = true;
			lblDateTo.Location = new System.Drawing.Point(97, 229);
			lblDateTo.Name = "lblDateTo";
			lblDateTo.Size = new System.Drawing.Size(33, 20);
			lblDateTo.TabIndex = 3;
			lblDateTo.Text = "To:";
			// 
			// pnl
			// 
			pnl.AutoSize = true;
			pnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			pnl.ColumnCount = 2;
			pnl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			pnl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			pnl.Controls.Add(this.listStocks, 0, 0);
			pnl.Controls.Add(this.btnOK, 0, 3);
			pnl.Controls.Add(this.btnCancel, 1, 3);
			pnl.Controls.Add(lblDateFrom, 0, 1);
			pnl.Controls.Add(this.pickDateFrom, 1, 1);
			pnl.Controls.Add(lblDateTo, 0, 2);
			pnl.Controls.Add(this.pickDateTo, 1, 2);
			pnl.Location = new System.Drawing.Point(12, 12);
			pnl.Name = "pnl";
			pnl.RowCount = 4;
			pnl.RowStyles.Add(new System.Windows.Forms.RowStyle());
			pnl.RowStyles.Add(new System.Windows.Forms.RowStyle());
			pnl.RowStyles.Add(new System.Windows.Forms.RowStyle());
			pnl.RowStyles.Add(new System.Windows.Forms.RowStyle());
			pnl.Size = new System.Drawing.Size(290, 326);
			pnl.TabIndex = 0;
			// 
			// listStocks
			// 
			this.listStocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			pnl.SetColumnSpan(this.listStocks, 2);
			this.listStocks.FormattingEnabled = true;
			this.listStocks.ItemHeight = 20;
			this.listStocks.Location = new System.Drawing.Point(3, 3);
			this.listStocks.MaximumSize = new System.Drawing.Size(350, 200);
			this.listStocks.MinimumSize = new System.Drawing.Size(250, 150);
			this.listStocks.Name = "listStocks";
			this.listStocks.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listStocks.Size = new System.Drawing.Size(284, 184);
			this.listStocks.TabIndex = 0;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnOK.AutoSize = true;
			this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnOK.Location = new System.Drawing.Point(10, 266);
			this.btnOK.Margin = new System.Windows.Forms.Padding(10);
			this.btnOK.Name = "btnOK";
			this.btnOK.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
			this.btnOK.Size = new System.Drawing.Size(113, 50);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "Show";
			this.btnOK.UseVisualStyleBackColor = false;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnCancel.AutoSize = true;
			this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(156, 266);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(10);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
			this.btnCancel.Size = new System.Drawing.Size(111, 50);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Back";
			this.btnCancel.UseVisualStyleBackColor = false;
			// 
			// pickDateFrom
			// 
			this.pickDateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.pickDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.pickDateFrom.Location = new System.Drawing.Point(136, 193);
			this.pickDateFrom.Name = "pickDateFrom";
			this.pickDateFrom.Size = new System.Drawing.Size(151, 27);
			this.pickDateFrom.TabIndex = 2;
			// 
			// pickDateTo
			// 
			this.pickDateTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.pickDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.pickDateTo.Location = new System.Drawing.Point(136, 226);
			this.pickDateTo.Name = "pickDateTo";
			this.pickDateTo.Size = new System.Drawing.Size(151, 27);
			this.pickDateTo.TabIndex = 4;
			// 
			// FormHistoryFilter
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(360, 388);
			this.ControlBox = false;
			this.Controls.Add(pnl);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormHistoryFilter";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Filter history";
			pnl.ResumeLayout(false);
			pnl.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ListBox listStocks;
		private System.Windows.Forms.DateTimePicker pickDateFrom;
		private System.Windows.Forms.DateTimePicker pickDateTo;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}