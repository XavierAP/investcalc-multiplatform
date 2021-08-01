using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	/// <summary>Dialog common to all operations, accepting a selection of stock, number of shares, and money amount.</summary>
	internal partial class FormOp : Form, OperationDialog
	{
		readonly Operation operation;
		readonly OperationRecorder dataWriter;
		readonly PortfolioView dataView;

		internal FormOp(Operation op, PortfolioData dataModel, PortfolioView dataView)
		{
			this.operation = op;
			this.dataWriter = new OperationRecorder(this, dataModel);
			this.dataView = dataView;

			InitializeComponent();
			InitializeUI();

			listStocks.SelectedIndexChanged += OnStockSelectionChanged;
			listStocks.Items.AddRange(dataModel.Keys.ToArray());

			FormClosing += ConfirmClose;
		}

		private void InitializeUI()
		{
			lblTotal.Text = $"Total ({NumberFormatInfo.CurrentInfo.CurrencySymbol})";
			Text = operation.Text;
			txtStock.Enabled = operation.SharesChange && !operation.SharesMinus; // Buying/acquiring is the only possible operation on a stock that we don't yet own.
			numShares.Enabled = operation.SharesChange;
			pickDate.Value = DateTime.Now.Date;
		}


		private void OnStockSelectionChanged(object sender, EventArgs ea)
		{
			txtStock.Text = listStocks.SelectedIndex < 0 ? null : listStocks.SelectedItem.ToString();
		}
		

		private async void ConfirmClose(object sender, FormClosingEventArgs ea)
		{
			if(DialogResult != DialogResult.OK) return; // Cancel: just go on closing
			bool ok = await dataWriter.Record(operation);
			ea.Cancel = !ok;
		}

		#region OperationDialog implementation
		
		PortfolioView OperationDialog.PortfolioView => dataView;

		string OperationDialog.StockName
		{
			get => txtStock.Text;
			set => txtStock.Text = value;
		}
		double OperationDialog.Shares => (double)numShares.Value;
		double OperationDialog.TotalMoney => (double)numTotal.Value;
		DateTime OperationDialog.Date => pickDate.Value;
		string OperationDialog.Comment => txtComment.Text;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		async Task<bool> OperationDialog.PromptConfirmation(string message)
		{
			return DialogResult.Yes == MessageBox.Show(message, "Please confirm",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
		}
		async Task OperationDialog.PromptError(string message)
		{
			MessageBox.Show(message, "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

		void OperationDialog.OnErrorEmptyStockName() => listStocks.Focus();
		void OperationDialog.OnErrorZeroMoney() => numTotal.Focus();
		void OperationDialog.OnErrorZeroShares() => numShares.Focus();
		void OperationDialog.OnErrorSellingMoreSharesThanOwned(double owned)
		{
			numShares.Value = (decimal)owned;
			numShares.Focus();
		}

		#endregion
	}
}
