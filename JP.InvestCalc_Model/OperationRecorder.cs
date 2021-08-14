using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace JP.InvestCalc
{
	public interface OperationDialog
	{
		PortfolioView PortfolioView { get; }

		string StockName { get; set; }
		double Shares { get; }
		double TotalMoney { get; }
		DateTime Date { get; }
		string Comment { get; }

		Task<bool> PromptConfirmation(string message);

		Task PromptError(string message);

		void OnErrorEmptyStockName();
		void OnErrorSellingMoreSharesThanOwned(double shares);
		void OnErrorZeroMoney();
		void OnErrorZeroShares();
	}

	public class OperationRecorder
	{
		private readonly OperationDialog dialog;
		private readonly PortfolioData data;
		
		private bool OnlyAddingStockToPortfolioNotBuyingAnyShares = false;

		public OperationRecorder(OperationDialog dialog, PortfolioData data)
		{
			this.dialog = dialog;
			this.data = data;
		}

		public async Task<bool> Record(Operation op)
		{
			if(await HasError(op))
				return false;

			if(!await ConfirmByUser(op))
				return false;

			var shares = op.SharesMinus ? -dialog.Shares : dialog.Shares;
			var money = op.MoneyMinus ? -dialog.TotalMoney : dialog.TotalMoney;
			data.AddShares(dialog.PortfolioView, dialog.StockName,
				shares, money, dialog.Date, dialog.Comment);

			return true;
		}

		private async Task<bool> HasError(Operation op)
		{
			if(string.IsNullOrEmpty(dialog.StockName))
			{
				await dialog.PromptError("You must select a stock name.");
				dialog.OnErrorEmptyStockName();
				return true;
			}
			dialog.StockName = dialog.StockName.Trim();

			if(dialog.Shares < 0 || dialog.TotalMoney < 0)
			{
				await dialog.PromptError("Do not enter negative numbers.");
				return true;
			}
			if(op.SharesChange && dialog.Shares == 0)
			{
				if(op == Operation.Buy && !data.ContainsKey(dialog.StockName))
				{
					OnlyAddingStockToPortfolioNotBuyingAnyShares = true;
					return false;
				}
				await dialog.PromptError("You must enter an amount of shares.");
				dialog.OnErrorZeroShares();
				return true;
			}
			if(!op.SharesChange && dialog.TotalMoney == 0)
			{
				await dialog.PromptError("You must enter a money amount.");
				dialog.OnErrorZeroMoney();
				return true;
			}
			if(data.TryGetValue(dialog.StockName, out var owned))
			{
				if(op.SharesMinus && dialog.Shares > owned.Shares)
				{
					await dialog.PromptError("Cannot sell more shares than you own.");
					dialog.OnErrorSellingMoreSharesThanOwned(owned.Shares);
					return true;
				}
			}
			return false;
		}

		private async Task<bool> ConfirmByUser(Operation op)
		{
			if(OnlyAddingStockToPortfolioNotBuyingAnyShares)
				return await dialog.PromptConfirmation("'Buying' 0 shares will add the stock to the portfolio with no position (so that its price can be followed). As there is no actual operation to register, any comment entered will be discarded.");
			
			Debug.Assert(op.SharesChange || dialog.Shares == 0);

			var msg = new StringBuilder(op.Text);
			if(op.SharesChange)
				msg.Append($" {dialog.Shares} of");

			msg.AppendLine();
			msg.AppendLine(dialog.StockName);
			msg.Append("for ").AppendLine(dialog.TotalMoney.ToString("C"));
			msg.Append($"on {dialog.Date.ToLongDateString()}?");

			return await dialog.PromptConfirmation(msg.ToString());
		}
	}
}
