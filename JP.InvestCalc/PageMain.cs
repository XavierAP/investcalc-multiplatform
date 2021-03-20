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
		
		readonly static RowDefinition layoutRowHeader = new RowDefinition { Height = GridLength.Auto };
		readonly static RowDefinition layoutRowOther = new RowDefinition { Height = GridLength.Auto };
		readonly static ColumnDefinitionCollection layoutCols;
		readonly static LayoutOptions layoutFillOption = new LayoutOptions(LayoutAlignment.Fill, true);
		readonly static LayoutOptions layoutEndOption  = new LayoutOptions(LayoutAlignment.End, false);

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

			int irow = 0;
			// Header: stock name and number of shares.
			stockGrid.RowDefinitions.Add(layoutRowHeader);
			stockGrid.Children.Add(btn = new Button
			{
				Text = name,
				FontAttributes = FontAttributes.Bold,
				BackgroundColor = Color.LightBlue,
				BorderColor = Color.Blue,
				BorderWidth = 1,
				CornerRadius = 10,
			}, 0, 3, irow, irow + 1);
			stockGrid.Children.Add(fields.Shares = new Label
			{
				Text = shares.ToString(),
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				FontAttributes = FontAttributes.Bold,
				BackgroundColor = Color.LightBlue,
			}, 3, 4, irow, irow + 1);

			btn.Clicked += (s, e) => PromptStockActions(name);

			++irow;
			// Left column, double height: price.
			stockGrid.RowDefinitions.Add(layoutRowOther);
			stockGrid.RowDefinitions.Add(layoutRowOther);
			stockGrid.Children.Add(new Label { Text = "Price:", HorizontalTextAlignment = TextAlignment.End },
				0, 1, irow, irow + 2);
			stockGrid.Children.Add(fields.Price = new Entry { Keyboard = Keyboard.Numeric },
				1, 2, irow, irow + 2);

			int icol = 2;
			// Right column, down from top: total value, yearly return.
			stockGrid.Children.Add(new Label { Text = "Value:", HorizontalTextAlignment = TextAlignment.End },
				icol, irow);
			stockGrid.Children.Add(fields.TotalValue = new Label { },
				icol + 1, irow);
			stockGrid.Children.Add(new Label { Text = "Yearly:", HorizontalTextAlignment = TextAlignment.End },
				icol, ++irow);
			stockGrid.Children.Add(fields.Return = new Label { },
				icol + 1, irow);

			if(returnPer1Yearly.HasValue)
				fields.Return.Text = FormatPerCent(returnPer1Yearly);

			fields.Price.Completed += (s,e) => OnPriceChanged(name);

			stockIndex.Add(name, fields);
		}

		public void SetStockFigures(Stock stk, double? returnPer1Yearly)
		{
			var fields = stockIndex[stk.Name];
			fields.Shares.Text = stk.Shares.ToString();
			if(stk.Price.HasValue)
			{
				fields.Price.Text = stk.Price.ToString();
				fields.TotalValue.Text = FormatMoney(stk.Price * stk.Shares);
			}
			else
			{
				fields.Price.Text =
				fields.TotalValue.Text = null;
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

			if(double.TryParse(fields.Price.Text, out var price))
			{
				var stk = model.Portfolio.GetStock(stockName);
				fields.TotalValue.Text = FormatMoney(price * stk.Shares);
				fields.Return.Text = FormatPerCent(
					model.Calculator.CalcReturn(stk.Name, stk.Shares, price));
			}
			else
				fields.TotalValue.Text = null;
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
					model.Data.SetFetchCode(stockName,
						await DisplayPromptAsync(stockName, "Enter fetch code"));
					RefreshPortfolio();
					break;

				case "Edit stock name":
					model.Data.SetStockName(stockName,
						await DisplayPromptAsync(stockName, "Enter a new name",
							initialValue: stockName));
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

		private static string FormatMoney(double? value) => value.Value.ToString("N2");

		private static string FormatPerCent(double? per1) => per1.Value.ToString("P" + Config.PrecisionPerCent);

		private Task DisplayError(Exception err) => DisplayAlert("ERROR! " + err.GetType().Name, err.Message, "OK");
	}
}
