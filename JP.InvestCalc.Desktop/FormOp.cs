using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	/// <summary>Dialog common to all operations, accepting a selection of stock, number of shares, and money amount.</summary>
	internal partial class FormOp :Form
	{
		private readonly Operation opCurrent;
		private readonly Dictionary<string, double> portfolio;

		public string StockName   => txtStock.Text;
		public double Shares  => (double)(opCurrent.SharesMinus ? -numShares.Value : numShares.Value);
		public double Total   => (double)(opCurrent.MoneyMinus  ? -numTotal.Value  : numTotal.Value );
		public DateTime Date  => pickDate.Value;
		public string Comment => txtComment.Text;

		internal FormOp(Operation op, Dictionary<string, double> portfolio)
		{
			Debug.Assert(op != null && portfolio != null);
			this.opCurrent = op;
			this.portfolio = portfolio;

			InitializeComponent();
			lblTotal.Text = $"Total ({NumberFormatInfo.CurrentInfo.CurrencySymbol})";
			
			pickDate.Value = DateTime.Now.Date;

			listStocks.SelectedIndexChanged += StockChangedSelect;
			listStocks.Items.AddRange(portfolio.Keys.ToArray());

			FormClosing += ConfirmClose;
			
			// Adapt interface to the type of operation:
			Text = op.Text;
			txtStock.Enabled = op.SharesChange && !op.SharesMinus; // Buying/acquiring is the only possible operation on a stock that we don't yet own.
			numShares.Enabled = op.SharesChange;
		}


		/// <summary>Updates txtStock when the user selects from listStocks.</summary>
		private void StockChangedSelect(object sender, EventArgs ea)
		{
			txtStock.Text = listStocks.SelectedIndex < 0 ? null : listStocks.SelectedItem.ToString();
		}
		

		/// <summary>Handles the Closing event and requests confirmation before proceeding with the operation.</summary>
		private void ConfirmClose(object sender, FormClosingEventArgs ea)
		{
			if(DialogResult != DialogResult.OK) return; // Cancel: just go on closing
			
			txtStock.Text = txtStock.Text.Trim();

			if(HasError())
			{
				ea.Cancel = true;
				return;
			}

			if(portfolio.ContainsKey(txtStock.Text))
			{
				var max = (decimal)(double)portfolio[txtStock.Text];
				if(opCurrent.SharesMinus && numShares.Value > max)
				{
					MessageBox.Show(this, "Cannot sell more shares than you own.", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					numShares.Value = max;
					ea.Cancel = true;
					return;
				}
			}

			// Compose summary (confirmation) message:
			var msg = new StringBuilder(opCurrent.Text);
			Debug.Assert(opCurrent.SharesChange || numShares.Value == 0);
			if(opCurrent.SharesChange) msg.Append($" {numShares.Value} of");
			msg.AppendLine();
			msg.AppendLine(txtStock.Text);
			msg.Append("for ").AppendLine(numTotal.Value.ToString("C"));
			msg.Append($"on {pickDate.Value.ToLongDateString()}?");

			var ans = MessageBox.Show(this, msg.ToString(), "Please confirm",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

			if(ans != DialogResult.Yes)
				ea.Cancel = true;
		}

		/// <summary>Returns false in case of user input error; otherwise shows an explanatory error popup, and returns true.</summary>
		private bool HasError()
		{
			if(string.IsNullOrEmpty(txtStock.Text))
			{
				MessageBox.Show(this, $"You must select{(txtStock.Enabled ? " or enter" : null)} a stock name.", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				listStocks.Focus();
				return true;
			}
			if(opCurrent.SharesChange && numShares.Value == 0)
			{
				MessageBox.Show(this, "You must enter an amount of shares.", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				numShares.Focus();
				return true;
			}
			if(!opCurrent.SharesChange && numTotal.Value == 0)
			{
				MessageBox.Show(this, "You must enter a money amount.", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				numTotal.Focus();
				return true;
			}
			return false;
		}
	}
}
