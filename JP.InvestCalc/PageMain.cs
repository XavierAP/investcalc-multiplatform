using JP.Utils;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Data.Common;

namespace JP.InvestCalc
{
	internal sealed class PageMain : ContentPage, PortfolioView
	{
		private readonly ModelGateway model = new ModelGateway(GetDataFolder(), GetPriceAPILicense());

		private readonly StackLayout containerLayout = new StackLayout();

		private readonly Dictionary<string, (Label Shares, Entry Price, Label Value, Label Return)> stockIndex
			= new Dictionary<string, (Label Shares, Entry Price, Label Value, Label Return)>();

		private readonly static RowDefinition layoutRow = new RowDefinition { Height = GridLength.Auto };
		private readonly static ColumnDefinitionCollection layoutCols;

		const string Pad = " ";

		static PageMain()
		{
			var layoutCol = new ColumnDefinition { Width = GridLength.Star };
			layoutCols = new ColumnDefinitionCollection { layoutCol, layoutCol, layoutCol, layoutCol };

			ExperimentalFeatures.Enable("ShareFileRequest_Experimental");
		}

		public PageMain()
		{
			var pageStack = new StackLayout();
			this.Content = pageStack;
			pageStack.BackgroundColor = Color.AliceBlue;

			var scroll = new ScrollView();
			pageStack.Children.Add(scroll);
			scroll.Padding = 10;
			scroll.Content = containerLayout;

			RefreshPortfolio();
		}

		private void RefreshPortfolio()
		{
			stockIndex.Clear();
			containerLayout.Children.Clear();
			try
			{
				model.Portfolio.Load(this);
			}
			catch(DbException)
			{
				File.Delete(model.Data.FilePath);
				Environment.Exit(1);
			}
			AddButtons();
		}

		public void InvokeOnUIThread(Action action) => MainThread.BeginInvokeOnMainThread(action);

		public void AddStock(string name, double shares, double? returnPer1Yearly)
		{
			var stockGrid = new Grid { ColumnDefinitions = layoutCols };
			containerLayout.Children.Add(stockGrid);
			(Label Shares, Entry Price, Label Value, Label Return) fields;

			int irow = 0;
			// Header: stock name and number of shares.
			stockGrid.RowDefinitions.Add(layoutRow);
			stockGrid.Children.Add(new Label
			{
				Text = Pad + name,
				FontAttributes = FontAttributes.Bold
			}, 0, 3, irow, irow + 1);
			stockGrid.Children.Add(fields.Shares = new Label
			{
				Text = shares.ToString() + Pad,
				HorizontalTextAlignment = TextAlignment.End
			}, 3, 4, irow, irow + 1);
			foreach(var it in stockGrid.Children)
				it.BackgroundColor = Color.LightBlue;

			++irow;
			// Left column, double height: price.
			stockGrid.RowDefinitions.Add(layoutRow);
			stockGrid.RowDefinitions.Add(layoutRow);
			stockGrid.Children.Add(new Label { Text = "Price:", HorizontalTextAlignment = TextAlignment.End },
				0, 1, irow, irow + 2);
			stockGrid.Children.Add(fields.Price = new Entry { Keyboard = Keyboard.Numeric },
				1, 2, irow, irow + 2);

			int icol = 2;
			// Right column, down from top: total value, yearly return.
			stockGrid.Children.Add(new Label { Text = "Value:", HorizontalTextAlignment = TextAlignment.End },
				icol, irow);
			stockGrid.Children.Add(fields.Value = new Label { },
				icol + 1, irow);
			stockGrid.Children.Add(new Label { Text = "Yearly:", HorizontalTextAlignment = TextAlignment.End },
				icol, ++irow);
			stockGrid.Children.Add(fields.Return = new Label { },
				icol + 1, irow);

			if(returnPer1Yearly.HasValue)
				fields.Return.Text = FormatPerCent(returnPer1Yearly);

			fields.Price.TextChanged += Price_Changed;

			stockIndex.Add(name, fields);
		}

		public void SetStockFigures(Stock stk, double? returnPer1Yearly)
		{
			var fields = stockIndex[stk.Name];
			fields.Shares.Text = stk.Shares.ToString() + Pad;
			if(stk.Price.HasValue)
			{
				fields.Price.Text = stk.Price.ToString();
				fields.Value.Text = FormatMoney(stk.Price * stk.Shares);
			}
			else
			{
				fields.Price.Text =
				fields.Value.Text = null;
			}
			if(returnPer1Yearly.HasValue)
				fields.Return.Text = FormatPerCent(returnPer1Yearly);
			else
				fields.Return.Text = null;
		}

		private void AddButtons()
		{
			var commandGrid = new Grid { ColumnDefinitions = layoutCols };
			containerLayout.Children.Add(commandGrid);

			int icol, irow;
			Button btn;

			commandGrid.RowDefinitions.Add(layoutRow);
			commandGrid.Children.Add(new Button { Text = "Buy" },
				icol = 0, irow = 0);
			commandGrid.Children.Add(new Button { Text = "Sell" },
				++icol, irow);
			commandGrid.Children.Add(new Button { Text = "Div." },
				++icol, irow);
			commandGrid.Children.Add(new Button { Text = "Cost" },
				++icol, irow);

			commandGrid.RowDefinitions.Add(layoutRow);

			commandGrid.Children.Add(btn = new Button { Text = "History..." },
				0, icol = 2, ++irow, irow + 1);
			btn.Clicked += FooHistory;

			commandGrid.Children.Add(btn = new Button { Text = "Options" },
				icol, 4, irow, irow + 1);
			btn.Clicked += PromptOptions;
		}

		private void FooHistory(object sender, EventArgs ea)
		{
			var flows = model.Data.GetFlowEditor().GetFlowDetailsOrdered(new[] { "ASML" },
				new DateTime(2000, 1, 1), DateTime.Now
				).ToArray();
		}

		private void Price_Changed(object sender, EventArgs ea)
		{
			var priceInput = (Entry)sender;
			var fields = stockIndex.Values.Where(f => priceInput == f.Price).Single();

			if(double.TryParse(priceInput.Text, out var price))
				fields.Value.Text = FormatMoney(price * double.Parse(fields.Shares.Text));
			else
				fields.Value.Text = null;
		}

		private async void PromptOptions(object sender, EventArgs ea)
		{
			var option = await DisplayActionSheet("Options", "Cancel", null,
				"Export data file",
				"Import data file",
				"Price lookup symbols",
				"Price lookup license");
			switch(option)
			{
				case "Export data file": await ExportDataFile(); break;
				case "Import data file": await ImportDataFile(); break;
				case "Price lookup license": await PromptLicense(); break;

				case "Cancel":
				default:
					return;
			}
		}

		private Task ExportDataFile() => Share.RequestAsync(new ShareFileRequest
		{
			File = new ShareFile(model.Data.FilePath, "application/octet-stream")
		});

		private async Task ImportDataFile()
		{
			var file = await CrossFilePicker.Current.PickFile();
			if(file == null) return;
			using(var stream = file.GetStream())
			{
				var err = await Files.TryCopy(stream, model.Data.FilePath);
				if(err == null)
					RefreshPortfolio();
				else
					await DisplayError(err);
			}
		}

		private async Task PromptLicense()
		{
			string license = await DisplayPromptAsync("API license", "Enter your key from AlphaVantage.co:");
			if(license == null) return;

			File.WriteAllText(GetAPILicenseFileName(), license);
			Environment.Exit(0);
		}

		private static string GetPriceAPILicense()
		{
			var pathName = GetAPILicenseFileName();
			if(!File.Exists(pathName)) return null;

			return File.ReadAllText(pathName).Trim();
		}

		private static string GetAPILicenseFileName() => Path.Combine(GetDataFolder(), "api-license.txt");

		private static string GetDataFolder() => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

		private static string FormatMoney(double? value) => value.Value.ToString("N2");

		private static string FormatPerCent(double? per1) => per1.Value.ToString("P" + Config.PrecisionPerCent);

		private Task DisplayError(Exception err) => DisplayAlert("ERROR! " + err.GetType().Name, err.Message, "OK");
	}
}
