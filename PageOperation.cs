using Xamarin.Forms;

namespace JP.InvestCalc
{
	class PageOperation : CenteredDialogPage
	{
		public static PageOperation OnExistingStock(PortfolioData data, Operation op, string stockName)
			=> new PageOperation(data, op, stockName);

		public static PageOperation BuyNewStock(PortfolioData data)
			=> new PageOperation(data, Operation.Buy, null);

		readonly PortfolioData portfolio;
		readonly Entry? stockNameEntry;
		readonly Entry
			sharesEntry,
			totalMoneyEntry,
			commentEntry;

		private PageOperation(PortfolioData data, Operation op, string? stockName)
		{
			portfolio = data;

			AddTextRow(op.Text)
				.FontAttributes = FontAttributes.Bold;

			if(stockName != null)
				AddTextRow(stockName);
			else
			{
				stockNameEntry = AddEntryRow("Stock:").Entry;
				stockNameEntry.Keyboard = Keyboard.Text;
			}

			sharesEntry = AddEntryRow("Shares:").Entry;
			sharesEntry.Keyboard = Keyboard.Numeric;

			totalMoneyEntry = AddEntryRow("Total (€):").Entry;
			totalMoneyEntry.Keyboard = Keyboard.Numeric;

			commentEntry = AddEntryRow("Comment:").Entry;
			commentEntry.Keyboard = Keyboard.Numeric;
		}
	}
}
