namespace JP.InvestCalc
{
	partial class FormOp
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
			this.lblShares = new System.Windows.Forms.Label();
			this.lblTotal = new System.Windows.Forms.Label();
			this.pnlTab = new System.Windows.Forms.TableLayoutPanel();
			this.numTotal = new System.Windows.Forms.NumericUpDown();
			this.listStocks = new System.Windows.Forms.ListBox();
			this.txtStock = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.numShares = new System.Windows.Forms.NumericUpDown();
			this.grpComment = new System.Windows.Forms.GroupBox();
			this.txtComment = new System.Windows.Forms.TextBox();
			this.pickDate = new System.Windows.Forms.DateTimePicker();
			this.pnlTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numTotal)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numShares)).BeginInit();
			this.grpComment.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblShares
			// 
			this.lblShares.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblShares.AutoSize = true;
			this.lblShares.Location = new System.Drawing.Point(296, 13);
			this.lblShares.Margin = new System.Windows.Forms.Padding(10);
			this.lblShares.Name = "lblShares";
			this.lblShares.Size = new System.Drawing.Size(67, 20);
			this.lblShares.TabIndex = 2;
			this.lblShares.Text = "Shares:";
			// 
			// lblTotal
			// 
			this.lblTotal.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.lblTotal.AutoSize = true;
			this.lblTotal.Location = new System.Drawing.Point(293, 60);
			this.lblTotal.Name = "lblTotal";
			this.lblTotal.Size = new System.Drawing.Size(77, 20);
			this.lblTotal.TabIndex = 4;
			this.lblTotal.Text = "Total (€):";
			// 
			// pnlTab
			// 
			this.pnlTab.AutoSize = true;
			this.pnlTab.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlTab.ColumnCount = 3;
			this.pnlTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.pnlTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.pnlTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.pnlTab.Controls.Add(this.numTotal, 2, 1);
			this.pnlTab.Controls.Add(this.listStocks, 0, 1);
			this.pnlTab.Controls.Add(this.txtStock, 0, 0);
			this.pnlTab.Controls.Add(this.lblShares, 1, 0);
			this.pnlTab.Controls.Add(this.btnOK, 1, 4);
			this.pnlTab.Controls.Add(this.btnCancel, 2, 4);
			this.pnlTab.Controls.Add(this.numShares, 2, 0);
			this.pnlTab.Controls.Add(this.grpComment, 1, 3);
			this.pnlTab.Controls.Add(this.pickDate, 1, 2);
			this.pnlTab.Controls.Add(this.lblTotal, 1, 1);
			this.pnlTab.Location = new System.Drawing.Point(12, 12);
			this.pnlTab.Name = "pnlTab";
			this.pnlTab.RowCount = 5;
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.pnlTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.pnlTab.Size = new System.Drawing.Size(524, 292);
			this.pnlTab.TabIndex = 0;
			// 
			// numTotal
			// 
			this.numTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.numTotal.DecimalPlaces = 2;
			this.numTotal.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numTotal.Location = new System.Drawing.Point(383, 57);
			this.numTotal.Margin = new System.Windows.Forms.Padding(10);
			this.numTotal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numTotal.Name = "numTotal";
			this.numTotal.Size = new System.Drawing.Size(131, 27);
			this.numTotal.TabIndex = 5;
			this.numTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// listStocks
			// 
			this.listStocks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listStocks.FormattingEnabled = true;
			this.listStocks.ItemHeight = 20;
			this.listStocks.Location = new System.Drawing.Point(10, 57);
			this.listStocks.Margin = new System.Windows.Forms.Padding(10);
			this.listStocks.Name = "listStocks";
			this.pnlTab.SetRowSpan(this.listStocks, 4);
			this.listStocks.Size = new System.Drawing.Size(200, 225);
			this.listStocks.TabIndex = 1;
			// 
			// txtStock
			// 
			this.txtStock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtStock.Location = new System.Drawing.Point(10, 10);
			this.txtStock.Margin = new System.Windows.Forms.Padding(10);
			this.txtStock.Name = "txtStock";
			this.txtStock.Size = new System.Drawing.Size(200, 27);
			this.txtStock.TabIndex = 0;
			// 
			// btnOK
			// 
			this.btnOK.AutoSize = true;
			this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnOK.Location = new System.Drawing.Point(230, 218);
			this.btnOK.Margin = new System.Windows.Forms.Padding(10);
			this.btnOK.Name = "btnOK";
			this.btnOK.Padding = new System.Windows.Forms.Padding(30, 10, 30, 10);
			this.btnOK.Size = new System.Drawing.Size(133, 64);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "   OK   ";
			this.btnOK.UseVisualStyleBackColor = false;
			// 
			// btnCancel
			// 
			this.btnCancel.AutoSize = true;
			this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnCancel.Location = new System.Drawing.Point(383, 218);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(10);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(30, 10, 30, 10);
			this.btnCancel.Size = new System.Drawing.Size(131, 64);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			// 
			// numShares
			// 
			this.numShares.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.numShares.DecimalPlaces = 3;
			this.numShares.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numShares.Location = new System.Drawing.Point(383, 10);
			this.numShares.Margin = new System.Windows.Forms.Padding(10);
			this.numShares.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numShares.Name = "numShares";
			this.numShares.Size = new System.Drawing.Size(131, 27);
			this.numShares.TabIndex = 3;
			this.numShares.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// grpComment
			// 
			this.grpComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpComment.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlTab.SetColumnSpan(this.grpComment, 2);
			this.grpComment.Controls.Add(this.txtComment);
			this.grpComment.Location = new System.Drawing.Point(223, 130);
			this.grpComment.Name = "grpComment";
			this.grpComment.Size = new System.Drawing.Size(298, 75);
			this.grpComment.TabIndex = 7;
			this.grpComment.TabStop = false;
			this.grpComment.Text = "Comment:";
			// 
			// txtComment
			// 
			this.txtComment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtComment.Location = new System.Drawing.Point(3, 23);
			this.txtComment.MaxLength = 255;
			this.txtComment.Multiline = true;
			this.txtComment.Name = "txtComment";
			this.txtComment.Size = new System.Drawing.Size(292, 49);
			this.txtComment.TabIndex = 0;
			// 
			// pickDate
			// 
			this.pickDate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.pnlTab.SetColumnSpan(this.pickDate, 2);
			this.pickDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.pickDate.Location = new System.Drawing.Point(383, 97);
			this.pickDate.Name = "pickDate";
			this.pickDate.Size = new System.Drawing.Size(138, 27);
			this.pickDate.TabIndex = 6;
			// 
			// FormOp
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(570, 335);
			this.ControlBox = false;
			this.Controls.Add(this.pnlTab);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormOp";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = " ";
			this.pnlTab.ResumeLayout(false);
			this.pnlTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numTotal)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numShares)).EndInit();
			this.grpComment.ResumeLayout(false);
			this.grpComment.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listStocks;
		private System.Windows.Forms.TextBox txtStock;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.NumericUpDown numShares;
		private System.Windows.Forms.TextBox txtComment;
		private System.Windows.Forms.TableLayoutPanel pnlTab;
		private System.Windows.Forms.GroupBox grpComment;
		private System.Windows.Forms.DateTimePicker pickDate;
		private System.Windows.Forms.NumericUpDown numTotal;
		private System.Windows.Forms.Label lblShares;
		private System.Windows.Forms.Label lblTotal;
	}
}