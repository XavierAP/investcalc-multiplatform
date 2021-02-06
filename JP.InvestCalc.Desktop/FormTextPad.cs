using System;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal partial class FormTextPad :Form
	{
		public string Content => txt.Text;

		private readonly string[] headers;

		public FormTextPad(bool readOnly, string[] headers, string content)
		{
			InitializeComponent();

			this.headers = headers;
			const string textSeparator = ", "; // not the same used for values
			lblHeaders.Text = string.Join(textSeparator, headers);
			txt.ReadOnly = readOnly;
			if(content != null)
				txt.Text = content;

			if(readOnly)
			{
				txt.SelectAll();
				Clipboard.SetText(content);
				if(showHelpOutput)
				{
					Shown += PromptHelpOutput;
					showHelpOutput = false;
				}
			}
			else
			{
				FormClosing += ConfirmImport;
				if(showHelpInput)
				{
					Shown += PromptHelpInput;
					showHelpInput = false;
				}
			}
		}


		private static bool
			showHelpOutput = true,
			showHelpInput  = true;

		private void PromptHelpOutput(object sender, EventArgs ea)
		{
			MessageBox.Show(this, "CSV copied automatically into the system clipboard. You can paste directly into Excel.", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			Shown -= PromptHelpOutput;
		}

		private void PromptHelpInput(object sender, EventArgs ea)
		{
			MessageBox.Show(this, $@"Enter the CSV into this Window, then close it to continue importing.
Enter values for the {headers.Length} columns enumerated above; do not include price; comments are optional.
Use separator
«{Properties.Settings.Default.csvSeparator}»
(or change this separator in the .config file and restart the app).", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

			Shown -= PromptHelpInput;
		}


		private void ConfirmImport(object sender, FormClosingEventArgs ea)
		{
			if(string.IsNullOrWhiteSpace(Content))
			{
				DialogResult = DialogResult.Cancel;
				return;
			}

			var ans = MessageBox.Show(this, "Do you want to parse and import these data as CSV?", "Please confirm",
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

			if(ans == DialogResult.Cancel)
				ea.Cancel = true;
			if(ans == DialogResult.No)
				DialogResult = DialogResult.Cancel;
			else
				DialogResult = DialogResult.OK;
		}
	}
}
