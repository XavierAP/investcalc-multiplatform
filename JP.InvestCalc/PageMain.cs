using JP.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	internal sealed class PageMain : ContentPage, PortfolioView
	{
		private readonly ModelGateway model = new ModelGateway(GetDataFolder(), GetPriceAPILicense());

		readonly StackLayout mainLayout = new StackLayout();
		readonly StackLayout stocksLayout = new StackLayout();

		readonly Grid buttonsLayoutOnPortrait = new Grid
		{
			ColumnDefinitions = layoutCols,
			HorizontalOptions = layoutFillOption,
			VerticalOptions   = layoutEndOption,
		};
		readonly Grid buttonsLayoutOnLandscape = new Grid
		{
			HorizontalOptions = layoutEndOption,
			VerticalOptions   = layoutFillOption,
		};

		readonly Button
			btnBuy      = new Button(),
			btnHistory  = new Button { Text = "History" },
			btnOptions  = new Button { Text = "Options" };

		private readonly Dictionary<string, (Label Shares, Entry Price, Label TotalValue, Label Return)>
		stockIndex = new Dictionary<string, (Label Shares, Entry Price, Label TotalValue, Label Return)>();
		
		readonly static RowDefinition layoutRowHeaderHalf = new RowDefinition { Height = GridLength.Auto };
		readonly static RowDefinition layoutRowOther = new RowDefinition { Height = GridLength.Auto };
		readonly static ColumnDefinitionCollection layoutCols;
		readonly static LayoutOptions layoutFillOption = new LayoutOptions(LayoutAlignment.Fill, true);
		readonly static LayoutOptions layoutEndOption  = new LayoutOptions(LayoutAlignment.End, false);
		 
		const string Pad = " ";

		static PageMain()
		{
			var layoutCol = new ColumnDefinition { Width = GridLength.Star };
			layoutCols = new ColumnDefinitionCollection { layoutCol, layoutCol, layoutCol, layoutCol };

			ExperimentalFeatures.Enable("ShareFileRequest_Experimental");
		}

		public PageMain()
		{
			this.Content = mainLayout;
			mainLayout.BackgroundColor = Color.AliceBlue;

			var scroll = new ScrollView
			{
				HorizontalOptions = layoutFillOption,
				VerticalOptions   = layoutFillOption,
			};
			mainLayout.Children.Add(scroll);
			scroll.Padding = 10;
			scroll.Content = stocksLayout;
			
			SetButtonsLayoutForPortrait();
			SetButtonsLayoutForLandscape();
			SizeChanged += OnSizeChanged;

			btnOptions.Clicked += PromptOptions;

			RefreshPortfolio();
		}

		private void OnSizeChanged(object sender, EventArgs ea)
		{
			if(Width > Height)
				PageOrientation = Orientation.Landscape;
			else
				PageOrientation = Orientation.Portrait;
		}

		private Orientation PageOrientation
		{
			set
			{
				if(value == _PageOrientation)
					return;

				ResetButtonsLayout();

				if(value == Orientation.Landscape)
				{
					LayButtonsForLandscape();
					mainLayout.Orientation = StackOrientation.Horizontal;
				}
				else
				{
					LayButtonsForPortrait();
					mainLayout.Orientation = StackOrientation.Vertical;
				}

				_PageOrientation = value;
			}
		}
		private Orientation _PageOrientation = Orientation.NotSet;


		private void RefreshPortfolio()
		{
			stockIndex.Clear();
			stocksLayout.Children.Clear();
			try
			{
				model.Portfolio.Load(this);
			}
			catch(DbException)
			{
				File.Delete(model.Data.FilePath);
				Environment.Exit(1);
			}
		}

		public void InvokeOnUIThread(Action action) => MainThread.BeginInvokeOnMainThread(action);

		public void AddStock(string name, double shares, double? returnPer1Yearly)
		{
			var stockGrid = new Grid { ColumnDefinitions = layoutCols };
			stocksLayout.Children.Add(stockGrid);
			(Label Shares, Entry Price, Label TotalValue, Label Return) fields;
			Button btn;

			int irow = 0,
				icol = 0;
			// Header: stock name (left 3 cols by 2 rows) and value and number of shares (right col by 1 row each).
			stockGrid.RowDefinitions.Add(layoutRowHeaderHalf);
			stockGrid.RowDefinitions.Add(layoutRowHeaderHalf);
			stockGrid.Children.Add(btn = new Button
			{
				Text = name,
				FontAttributes = FontAttributes.Bold,
				BackgroundColor = Color.LightBlue,
				BorderColor = Color.Blue,
				BorderWidth = 1,
				CornerRadius = 10,
			}, 0, icol = 3, irow, irow + 2);
			stockGrid.Children.Add(fields.TotalValue = new Label
			{
				Text = FormatValueOnUnknownPrice(shares),
				HorizontalTextAlignment = TextAlignment.End,
				BackgroundColor = Color.LightBlue,
			}, icol, irow);
			stockGrid.Children.Add(fields.Shares = new Label
			{
				Text = FormatShares(shares),
				HorizontalTextAlignment = TextAlignment.End,
				BackgroundColor = Color.LightBlue,
			}, icol, ++icol, ++irow, ++irow);

			btn.Clicked += (s, e) => PromptStockActions(name);

			icol = 0;
			// Left column: price.
			stockGrid.RowDefinitions.Add(layoutRowOther);
			stockGrid.Children.Add(new Label { Text = "Price:", HorizontalTextAlignment = TextAlignment.End },
				icol, ++icol, irow, irow + 1);
			stockGrid.Children.Add(fields.Price = new Entry { Keyboard = Keyboard.Numeric },
				icol, ++icol, irow, irow + 1);

			// Right column: yearly return.
			stockGrid.Children.Add(new Label { Text = "Yearly:", HorizontalTextAlignment = TextAlignment.End },
				icol, irow);
			stockGrid.Children.Add(fields.Return = new Label(),
				++icol, irow);

			if(returnPer1Yearly.HasValue)
				fields.Return.Text = FormatPerCent(returnPer1Yearly);

			fields.Price.Completed += (s,e) => OnPriceChanged(name);

			stockIndex.Add(name, fields);
		}

		public void SetStockFigures(Stock stk, double? returnPer1Yearly)
		{
			var fields = stockIndex[stk.Name];
			fields.Shares.Text = FormatShares(stk.Shares);
			if(stk.Price.HasValue)
			{
				fields.Price.Text = stk.Price.ToString();
				fields.TotalValue.Text = FormatMoney(stk.Price * stk.Shares);
			}
			else
			{
				fields.Price.Text = null;
				fields.TotalValue.Text = FormatValueOnUnknownPrice(stk.Shares);
			}
			if(returnPer1Yearly.HasValue)
				fields.Return.Text = FormatPerCent(returnPer1Yearly);
			else
				fields.Return.Text = null;
		}

		
		private void SetButtonsLayoutForPortrait()
		{
			buttonsLayoutOnPortrait.RowDefinitions.Add(layoutRowOther);
		}

		private void SetButtonsLayoutForLandscape()
		{
			buttonsLayoutOnLandscape.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			var layoutRow = new RowDefinition { Height = GridLength.Star };
			for(int i = 0; i < 3; ++i)
				buttonsLayoutOnLandscape.RowDefinitions.Add(layoutRow);
		}

		private void ResetButtonsLayout()
		{
			buttonsLayoutOnPortrait.Children.Clear();
			buttonsLayoutOnLandscape.Children.Clear();
			mainLayout.Children.Remove(buttonsLayoutOnPortrait);
			mainLayout.Children.Remove(buttonsLayoutOnLandscape);
		}

		private void LayButtonsForPortrait()
		{
			btnBuy.Text = "Buy new";

			const int irow = 0;
			int icol;
			buttonsLayoutOnPortrait.Children.Add(btnBuy,
				0, icol = 2, irow, irow+1);
			buttonsLayoutOnPortrait.Children.Add(btnHistory,
				icol, irow);
			buttonsLayoutOnPortrait.Children.Add(btnOptions,
				++icol, irow);

			mainLayout.Children.Add(buttonsLayoutOnPortrait);
		}

		private void LayButtonsForLandscape()
		{
			btnBuy.Text = "Buy\nnew";

			const int icol = 0;
			int irow;
			buttonsLayoutOnLandscape.Children.Add(btnBuy,
				icol, irow = 0);
			buttonsLayoutOnLandscape.Children.Add(btnHistory,
				icol, ++irow);
			buttonsLayoutOnLandscape.Children.Add(btnOptions,
				icol, ++irow);

			Debug.Assert(irow + 1 == buttonsLayoutOnLandscape.RowDefinitions.Count);

			mainLayout.Children.Add(buttonsLayoutOnLandscape);
		}


		private void OnPriceChanged(string stockName)
		{
			var fields = stockIndex[stockName];

			var stk = model.Portfolio.GetStock(stockName);
			if(double.TryParse(fields.Price.Text, out var price))
			{
				fields.TotalValue.Text = FormatMoney(price * stk.Shares);
				fields.Return.Text = FormatPerCent(
					model.Calculator.CalcReturn(stk.Name, stk.Shares, price));
			}
			else
				fields.TotalValue.Text = FormatValueOnUnknownPrice(stk.Shares);
		}

		private async void PromptStockActions(string stockName)
		{
			var option = await DisplayActionSheet(stockName, "Cancel", null,
				Operation.Buy.Text,
				Operation.Sell.Text,
				Operation.Dividend.Text,
				Operation.Cost.Text,
				"Enter fetch code",
				"Edit stock name");
			switch(option)
			{
				case "Enter fetch code":
					string code = await DisplayPromptAsync(stockName, "Enter fetch code");
					if(null == code)
						break;
					model.Data.SetFetchCode(stockName,
						code);
					RefreshPortfolio();
					break;

				case "Edit stock name":
					string newName = await DisplayPromptAsync(stockName, "Enter a new name",
						initialValue: stockName);
					if(string.IsNullOrWhiteSpace(newName))
						break;
					model.Data.SetStockName(stockName, newName);
					RefreshPortfolio();
					break;
					
				case "Cancel":
				default:
					return;
			}
		}


		private async void PromptOptions(object sender, EventArgs ea)
		{
			var option = await DisplayActionSheet("Options", "Cancel", null,
				"Export data file",
				"Import data file",
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
			var file = await FilePicker.PickAsync();
			if(file == null) return;
			using(var stream = await file.OpenReadAsync())
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
			string license = await DisplayPromptAsync("API license", "Enter your key from AlphaVantage.co:",
				initialValue: model.ApiLicenseKey);

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

		private static string FormatMoney(double? value) => value.Value.ToString("N2") + Pad;
		private static string FormatPerCent(double? per1) => per1.Value.ToString("P" + Config.PrecisionPerCent.ToString());
		private static string FormatShares(double shares) => $"({shares.ToString()}){Pad}";
		
		private static string FormatValueOnUnknownPrice(double shares) => shares == 0 ? FormatMoney(0) : null;

		private Task DisplayError(Exception err) => DisplayAlert("ERROR! " + err.GetType().Name, err.Message, "OK");
	}
}
