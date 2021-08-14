using JP.Utils;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	class PageOperation : CenteredDialogPage, OperationDialog
	{
		public static PageOperation OnExistingStock(Operation op, string stockName,
			PortfolioData dataModel, PortfolioView dataView)
			=> new PageOperation(op, stockName, dataModel, dataView);

		public static PageOperation BuyNewStock(PortfolioData dataModel, PortfolioView dataView)
			=> new PageOperation(Operation.Buy, null, dataModel, dataView);
		
		readonly Operation operation;
		readonly OperationRecorder dataWriter;

		readonly TextViewUnion stockName;
		readonly DatePicker date;
		readonly Entry? shares;
		readonly Entry
			totalMoney,
			comment;

		private PageOperation(Operation op, string? stock,
			PortfolioData dataModel, PortfolioView dataView)
		{
			operation = op;
			dataWriter = new OperationRecorder(this, dataModel);
			PortfolioView = dataView;

			AddElement(new Label
			{
				Text = op.Text,
				FontAttributes = FontAttributes.Bold,
			});

			stockName = stock != null ?
				new TextViewUnion(new Label { Text = stock }) :
				new TextViewUnion(new Entry
				{
					Placeholder = "Stock name",
					Keyboard = Keyboard.Text,
				});
			AddElement(stockName.AsView);

			if (operation.SharesChange)
			{
				AddElement(shares = new Entry
				{
					Placeholder = "Shares",
					Keyboard = Keyboard.Numeric,
				});
			}
			AddElement(totalMoney = new Entry
			{
				Placeholder = "Total (€)",
				Keyboard = Keyboard.Numeric,
			});
			AddElement(date = new DatePicker());
			AddElement(comment = new Entry
			{
				Placeholder = "Comment",
				Keyboard = Keyboard.Text,
			});

			var button = new Button { Text = "OK" };
			button.Clicked += TryCommitAndClose;
			AddElement(button);

			button = new Button { Text = "Cancel" };
			button.Clicked += async (s,e) => await Close();
			AddElement(button);
		}

		private async void TryCommitAndClose(object sender, EventArgs ea)
		{
			bool ok = await dataWriter.Record(operation);
			if(ok) await Close();
		}

		private async Task Close() => await Navigation.PopModalAsync();

		#region OperationDialog implementation

		public PortfolioView PortfolioView { get; }
		
		string OperationDialog.StockName
		{
			get => stockName.Text;
			set => stockName.Text = value;
		}
		double OperationDialog.Shares => shares == null ? 0d : ParseNumber(shares.Text);
		double OperationDialog.TotalMoney => ParseNumber(totalMoney.Text);
		DateTime OperationDialog.Date => date.Date;
		string OperationDialog.Comment => comment.Text;

		Task<bool> OperationDialog.PromptConfirmation(string message)
		{
			return this.PromptConfirmation(message);
		}
		Task OperationDialog.PromptError(string message)
		{
			return DisplayAlert("Invalid", message, "OK");
		}

		void OperationDialog.OnErrorEmptyStockName() => stockName.AsView.Focus();
		void OperationDialog.OnErrorZeroMoney() => totalMoney.Focus();
		void OperationDialog.OnErrorZeroShares() => shares?.Focus();
		void OperationDialog.OnErrorSellingMoreSharesThanOwned(double owned)
		{
			if (shares == null) return;
			shares.Text = owned.ToString();
			shares.Focus();
		}

		#endregion

		private static double ParseNumber(string? text)
		{
			if(string.IsNullOrEmpty(text))
				return 0;
			else if(double.TryParse(text, out var num))
				return num;
			else
				return 0;
		}
	}
}
