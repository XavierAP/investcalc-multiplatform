using JP.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	class PageHistory : ContentPage
	{
		private const ushort nColumns = 6;

		readonly FlowEditor data;

		readonly List<long> databaseIds;
		readonly long[] databaseIdCache = new long[1];
		
		readonly StackLayout flipLayout = new StackLayout();
		readonly Grid table = new Grid();
		readonly Button btnDeleteLast = new Button { Text = "Delete last" };

		readonly RowDefinition rowLayout = new RowDefinition { Height = GridLength.Auto };

		public bool HasChanged { get; private set; } = false;
		private bool IsEmpty => databaseIds.Count < 1;

		public PageHistory(FlowEditor data, params string[] stockNames)
		{
			this.data = data;

			this.Content = flipLayout;
			flipLayout.Children.Add(new ScrollView
			{
				Content = table,
				Orientation = ScrollOrientation.Both,
				BackgroundColor = Format.BackgroundColor,
			});
			for(int i = 0; i < nColumns; i++)
				table.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			
			AddHeaders();
			var rows = data.GetFlowDetailsOrdered(stockNames, DateTime.MinValue, DateTime.Now);
			databaseIds = new List<long>(rows.Count);
			foreach(var row in rows)
				AddRow(row);

			new OrientationFlipBehavior(this).SetOrChanged += OnOrientationSetOrChanged;

			btnDeleteLast.Clicked += DoDelete;
		}

		private async void DoDelete(object sender, EventArgs args)
		{
			bool confirmed = await this.PromptConfirmation("Are you sure you want to delete the last record?");
			if(!confirmed) return;

			DeleteLastRecordFromDatabase();
			DeleteLastRecordFromUI();
			if(IsEmpty) await Close();
		}

		private async Task Close() => await Navigation.PopModalAsync();

		private void OnOrientationSetOrChanged(Orientation orientation)
		{
			if(Orientation.Portrait == orientation)
			{
				if(!IsEmpty)
					flipLayout.Children.Add(btnDeleteLast);
			}
			else
			{
				flipLayout.Children.Remove(btnDeleteLast);
			}
		}

		private void AddHeaders()
		{
			AddRow("Date", "Stock", "Shares", "Flow", "Price", "Comment");
			foreach(Label header in table.Children)
				header.FontAttributes = FontAttributes.Bold;
		}

		private void AddRow(in (long DatabaseId, DateTime Date, string StockName,
			double Shares, double Flow, double PriceAvg, string Comment) row )
		{
			AddRow(
				row.Date.ToShortDateString(),
				row.StockName,
				row.Shares.FormatShares(),
				row.Flow.FormatMoney(),
				row.PriceAvg.FormatMoney(),
				row.Comment);

			databaseIds.Add(row.DatabaseId);
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

			Debug.Assert(icol == nColumns);
		}

		private void DeleteLastRecordFromDatabase()
		{
			var lastIndex = databaseIds.Count - 1;
			databaseIdCache[0] = databaseIds[lastIndex];
			databaseIds.RemoveAt(lastIndex);
			data.DeleteFlows(databaseIdCache);
			HasChanged = true;
		}

		private void DeleteLastRecordFromUI()
		{
			var lastIndex = table.Children.Count - 1;
			while(typeof(Button) == table.Children[lastIndex].GetType())
				--lastIndex;

			var lastIndexToRemain = lastIndex - nColumns;
			for(var i = lastIndex; i > lastIndexToRemain; --i)
				table.Children.RemoveAt(i);

			var rows = table.RowDefinitions;
			rows.RemoveAt(rows.Count - 1);
		}
	}
}
