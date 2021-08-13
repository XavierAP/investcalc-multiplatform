using System;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	class PageHistory : ContentPage
	{
		readonly Grid table = new Grid();
		readonly RowDefinition rowLayout = new RowDefinition { Height = GridLength.Auto };

		public PageHistory(FlowEditor data, params string[] stockNames)
		{
			this.Content = new ScrollView
			{
				Content = table,
				Orientation = ScrollOrientation.Both,
				BackgroundColor = Format.BackgroundColor,
			};
			for(int i = 0; i < 6; i++)
				table.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			
			AddHeaders();
			foreach(var row in data.GetFlowDetailsOrdered(stockNames, DateTime.MinValue, DateTime.Now))
				AddRow(
					row.Date.ToShortDateString(),
					row.StockName,
					row.Shares.FormatShares(),
					row.Flow.FormatMoney(),
					row.PriceAvg.FormatMoney(),
					row.Comment);
		}

		private void AddHeaders()
		{
			AddRow("Date", "Stock", "Shares", "Flow", "Price", "Comment");
			foreach(Label header in table.Children)
				header.FontAttributes = FontAttributes.Bold;
		}

		private void AddRow(string date, string stockName, string shares, string flow, string price, string comment)
		{
			int irow = table.RowDefinitions.Count;
			int icol = 0;
			table.RowDefinitions.Add(rowLayout);

			table.Children.Add(new Label { Text = date },
				icol++, irow);
			table.Children.Add(new Label { Text = stockName },
				icol++, irow);
			table.Children.Add(new Label { Text = shares },
				icol++, irow);
			table.Children.Add(new Label { Text = flow },
				icol++, irow);
			table.Children.Add(new Label { Text = price },
				icol++, irow);
			table.Children.Add(new Label { Text = comment },
				icol++, irow);
		}
	}
}
