
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
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
			this.ButtonCancel = new System.Windows.Forms.Button();
			this.ButtonSave = new System.Windows.Forms.Button();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tableLayoutPanel1.Controls.Add(this.ButtonCancel, 1, 0);
			tableLayoutPanel1.Controls.Add(this.ButtonSave, 0, 0);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 338);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tableLayoutPanel1.Size = new System.Drawing.Size(398, 60);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// ButtonCancel
			// 
			this.ButtonCancel.AutoSize = true;
			this.ButtonCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ButtonCancel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ButtonCancel.Location = new System.Drawing.Point(219, 5);
			this.ButtonCancel.Margin = new System.Windows.Forms.Padding(20, 5, 20, 5);
			this.ButtonCancel.Name = "ButtonCancel";
			this.ButtonCancel.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
			this.ButtonCancel.Size = new System.Drawing.Size(159, 50);
			this.ButtonCancel.TabIndex = 1;
			this.ButtonCancel.TabStop = false;
			this.ButtonCancel.Text = "Cancel";
			this.ButtonCancel.UseVisualStyleBackColor = true;
			// 
			// ButtonSave
			// 
			this.ButtonSave.AutoSize = true;
			this.ButtonSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ButtonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ButtonSave.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ButtonSave.Location = new System.Drawing.Point(20, 5);
			this.ButtonSave.Margin = new System.Windows.Forms.Padding(20, 5, 20, 5);
			this.ButtonSave.Name = "ButtonSave";
			this.ButtonSave.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
			this.ButtonSave.Size = new System.Drawing.Size(159, 50);
			this.ButtonSave.TabIndex = 0;
			this.ButtonSave.TabStop = false;
			this.ButtonSave.Text = "Save";
			this.ButtonSave.UseVisualStyleBackColor = true;
			// 
			// FormStockData
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(398, 398);
			this.Controls.Add(tableLayoutPanel1);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 400);
			this.Name = "FormStockData";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Stock data";
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button ButtonSave;
		private System.Windows.Forms.Button ButtonCancel;
	}
}