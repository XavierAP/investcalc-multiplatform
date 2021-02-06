namespace JP.InvestCalc
{
	partial class FormTextPad
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
			this.txt = new System.Windows.Forms.TextBox();
			this.lblHeaders = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txt
			// 
			this.txt.AcceptsReturn = true;
			this.txt.AcceptsTab = true;
			this.txt.AllowDrop = true;
			this.txt.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txt.Location = new System.Drawing.Point(0, 40);
			this.txt.Margin = new System.Windows.Forms.Padding(4);
			this.txt.Multiline = true;
			this.txt.Name = "txt";
			this.txt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txt.Size = new System.Drawing.Size(782, 513);
			this.txt.TabIndex = 0;
			this.txt.WordWrap = false;
			// 
			// lblHeaders
			// 
			this.lblHeaders.AutoSize = true;
			this.lblHeaders.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblHeaders.Location = new System.Drawing.Point(0, 0);
			this.lblHeaders.Name = "lblHeaders";
			this.lblHeaders.Padding = new System.Windows.Forms.Padding(10);
			this.lblHeaders.Size = new System.Drawing.Size(20, 40);
			this.lblHeaders.TabIndex = 1;
			// 
			// FormTextPad
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(782, 553);
			this.Controls.Add(this.txt);
			this.Controls.Add(this.lblHeaders);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 300);
			this.Name = "FormTextPad";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "CSV pad";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txt;
		private System.Windows.Forms.Label lblHeaders;
	}
}