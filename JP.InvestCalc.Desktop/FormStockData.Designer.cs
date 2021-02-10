
namespace JP.InvestCalc
{
	partial class FormStockData
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
			this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.btnLicense = new System.Windows.Forms.Button();
			this.btnSearch = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.layoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// layoutPanel
			// 
			this.layoutPanel.AutoSize = true;
			this.layoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutPanel.ColumnCount = 2;
			this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.layoutPanel.Controls.Add(this.btnLicense, 0, 0);
			this.layoutPanel.Controls.Add(this.btnSearch, 1, 0);
			this.layoutPanel.Controls.Add(this.btnSave, 0, 2);
			this.layoutPanel.Controls.Add(this.btnCancel, 1, 2);
			this.layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutPanel.Location = new System.Drawing.Point(0, 0);
			this.layoutPanel.Name = "layoutPanel";
			this.layoutPanel.RowCount = 3;
			this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.layoutPanel.Size = new System.Drawing.Size(398, 398);
			this.layoutPanel.TabIndex = 0;
			// 
			// btnLicense
			// 
			this.btnLicense.AutoSize = true;
			this.btnLicense.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnLicense.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnLicense.Location = new System.Drawing.Point(3, 3);
			this.btnLicense.Name = "btnLicense";
			this.btnLicense.Size = new System.Drawing.Size(193, 30);
			this.btnLicense.TabIndex = 0;
			this.btnLicense.Text = "License key...";
			// 
			// btnSearch
			// 
			this.btnSearch.AutoSize = true;
			this.btnSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnSearch.Location = new System.Drawing.Point(202, 3);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(193, 30);
			this.btnSearch.TabIndex = 1;
			this.btnSearch.Text = "Search fetchCodes...";
			// 
			// btnSave
			// 
			this.btnSave.AutoSize = true;
			this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnSave.Location = new System.Drawing.Point(8, 340);
			this.btnSave.Margin = new System.Windows.Forms.Padding(8);
			this.btnSave.Name = "btnSave";
			this.btnSave.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
			this.btnSave.Size = new System.Drawing.Size(183, 50);
			this.btnSave.TabIndex = 3;
			this.btnSave.TabStop = false;
			this.btnSave.Text = "Save";
			// 
			// btnCancel
			// 
			this.btnCancel.AutoSize = true;
			this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnCancel.Location = new System.Drawing.Point(207, 340);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(8);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
			this.btnCancel.Size = new System.Drawing.Size(183, 50);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "Cancel";
			// 
			// FormStockData
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(398, 398);
			this.Controls.Add(this.layoutPanel);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 400);
			this.Name = "FormStockData";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Stock data";
			this.layoutPanel.ResumeLayout(false);
			this.layoutPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnLicense;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.TableLayoutPanel layoutPanel;
	}
}