using JP.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	class PageHistory : ContentPage
	{
		const ushort
			nColumns = 6,
			iColShares = 2,
			iColPrice = 4;

		readonly FlowEditor data;
		CsvProcessor csv = new CsvProcessor(GetCsvSeparator());
		readonly string tempCsvFile = Path.Combine(Path.GetTempPath(), "InvestCalc-history.csv");

		readonly List<long> databaseIds;
		
		readonly StackLayout flipLayout = new StackLayout();
		readonly Grid table = new Grid();
		readonly Grid buttonsLayout;

		readonly RowDefinition rowLayout = new RowDefinition { Height = GridLength.Auto };

		ushort nButtons = 0;

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

			buttonsLayout = CreateButtonsLayout();
			AddButton("Delete last", DoDelete).IsEnabled = !IsEmpty;
			AddButton("CSV", PromptCsvOptions);

			new OrientationFlipBehavior(this).SetOrChanged += OnOrientationSetOrChanged;

			Disappearing += (s,a) => File.Delete(tempCsvFile);
		}

		private async Task Close() => await Navigation.PopModalAsync();

		private void OnOrientationSetOrChanged(Orientation orientation)
		{
			if(Orientation.Portrait == orientation)
				flipLayout.Children.Add(buttonsLayout);
			else
				flipLayout.Children.Remove(buttonsLayout);
		}
		
		private static Grid CreateButtonsLayout()
		{
			var layout = new Grid();
			layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			var layoutCol = new ColumnDefinition { Width = GridLength.Star };
			for(int c = 0; c < 2; c++)
				layout.ColumnDefinitions.Add(layoutCol);

			return layout;
		}

		private Button AddButton(string text, EventHandler clicked)
		{
			var btn = new Button { Text = text };
			buttonsLayout.Children.Add(btn, nButtons++, 0);
			btn.Clicked += clicked;
			return btn;
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
			Debug.Assert(icol == iColShares);
			table.Children.Add(new Label { Text = shares },
				icol++, irow);
			table.Children.Add(new Label { Text = flow },
				icol++, irow);
			Debug.Assert(icol == iColPrice);
			table.Children.Add(new Label { Text = price },
				icol++, irow);
			table.Children.Add(new Label { Text = comment },
				icol++, irow);

			Debug.Assert(icol == nColumns);
		}

		#region Delete button

		private async void DoDelete(object sender, EventArgs args)
		{
			bool confirmed = await this.PromptConfirmation("Are you sure you want to delete the last record?");
			if(!confirmed) return;

			DeleteLastRecordFromDatabase();
			DeleteLastRecordFromUI();
			if(IsEmpty) await Close();
		}

		private void DeleteLastRecordFromDatabase()
		{
			var lastIndex = databaseIds.Count - 1;
			databaseIdCache[0] = databaseIds[lastIndex];
			databaseIds.RemoveAt(lastIndex);
			data.DeleteFlows(databaseIdCache);
			HasChanged = true;
		}
		readonly long[] databaseIdCache = new long[1];

		private void DeleteLastRecordFromUI()
		{
			var lastIndex = table.Children.Count - 1;
			var lastIndexToRemain = lastIndex - nColumns;
			for(var i = lastIndex; i > lastIndexToRemain; --i)
				table.Children.RemoveAt(i);

			var rows = table.RowDefinitions;
			rows.RemoveAt(rows.Count - 1);
		}

		#endregion

		#region CSV menu button

		private async void PromptCsvOptions(object sender, EventArgs args)
		{
			var option = await DisplayActionSheet("CSV", "Cancel", null,
				"Export",
				"Import",
				"Configure");
			switch(option)
			{
				case "Export": await ExportCsvFile(); break;
				case "Import": await ImportCsvFile(); break;
				case "Configure": await PromptCsvOptionConfig(); break;

				case "Cancel":
				default:
					break;
			}
		}

		private async Task ExportCsvFile()
		{
			using(var writer = new StreamWriter(tempCsvFile))
			{
				csv.Synthesize(writer, GetCsvData());
			}
			await Share.RequestAsync(new ShareFileRequest
			{
				File = new ShareFile(tempCsvFile, "text/csv"),
			});
		}

		private IEnumerable<List<string>> GetCsvData()
		{
			const int nValues = nColumns - 1;
			var row = new List<string>(nValues);
			for(int i = nColumns; // Don't start at 0: skip headers
				i < table.Children.Count;
				i += nColumns, row.Clear() )
			{
				for(int c = 0; c < nColumns; c++)
				{
					if(iColPrice == c) continue; // price = flow/shares, displayed for info
			
					var text = (table.Children[i + c] as Label).Text;

					if(iColShares == c)
						row.Add(text.ReverseFormatShares());
					else
						row.Add(text);
				}
				Debug.Assert(nValues == row.Count);
				yield return row;
			}
		}
		private async Task ImportCsvFile()
		{
			var file = await FilePicker.PickAsync();
			if(file == null) return;

			int n;
			try {
				n = data.ImportFlows(File.ReadAllText(file.FullPath), csv);
			}
			catch(DataException err)
			{
				await this.DisplayError(err);
				return;
			}
			if(n < 1)
				await DisplayAlert(null, $"No rows found in file{Environment.NewLine}{file.FileName}", "OK");
			else
			{
				HasChanged = true;
				await DisplayAlert(null, n + " operations imported.", "OK");
				await Close();
			}
		}

		private async Task PromptCsvOptionConfig()
		{
			string separator = await DisplayPromptAsync("CSV", "Enter separator:");

			if(null == separator) return;

			if(0 == separator.Length)
			{
				await DisplayAlert(null, "Separator can't be empty.", "OK");
				return;
			}
			await SetCsvSeparator(separator);
		}

		private static string GetCsvSeparator()
		{
			if(Application.Current.Properties.TryGetValue("CsvSeparator", out object sep))
				return sep as string;
			else
				return Config.DefaultCsvSeparator;
		}
		private async Task SetCsvSeparator(string separator)
		{
			this.csv = new CsvProcessor(separator);
			Application.Current.Properties["CsvSeparator"] = separator;
			await Application.Current.SavePropertiesAsync();
		}

		#endregion
	}
}
