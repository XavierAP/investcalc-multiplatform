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
using System.Diagnostics;

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
			btnBuy      = new Button { Text = "Buy" },
			btnSell     = new Button { Text = "Sell" },
			btnDividend = new Button { Text = "Div." },
			btnCost     = new Button { Text = "Cost" },
			btnHistory  = new Button { Text = "History" },
			btnOptions  = new Button { Text = "Options" };

		private readonly Dictionary<string, (Label Shares, Entry Price, Label TotalValue, Label Return)>
		stockIndex = new Dictionary<string, (Label Shares, Entry Price, Label TotalValue, Label Return)>();

		readonly static RowDefinition layoutRow = new RowDefinition { Height = GridLength.Auto };
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

			btnHistory.Clicked += OpenHistory;
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
			fields.Shares.Text = stk.Shares.ToString() + Pad;
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
			for(int i = 0; i < 2; ++i)
				buttonsLayoutOnPortrait.RowDefinitions.Add(layoutRow);
		}

		private void SetButtonsLayoutForLandscape()
		{
			buttonsLayoutOnLandscape.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			var layoutRow = new RowDefinition { Height = GridLength.Star };
			for(int i = 0; i < 6; ++i)
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
			mainLayout.Children.Add(buttonsLayoutOnPortrait);

			int icol, irow;
			buttonsLayoutOnPortrait.Children.Add(btnBuy,
				icol = 0, irow = 0);
			buttonsLayoutOnPortrait.Children.Add(btnSell,
				++icol, irow);
			buttonsLayoutOnPortrait.Children.Add(btnDividend,
				++icol, irow);
			buttonsLayoutOnPortrait.Children.Add(btnCost,
				++icol, irow);
			buttonsLayoutOnPortrait.Children.Add(btnHistory,
				0, icol = 2, ++irow, irow + 1);
			buttonsLayoutOnPortrait.Children.Add(btnOptions,
				icol, 4, irow, irow + 1);
		}

		private void LayButtonsForLandscape()
		{
			mainLayout.Children.Add(buttonsLayoutOnLandscape);
			
			const int icol = 0;
			int irow;
			buttonsLayoutOnLandscape.Children.Add(btnBuy,
				icol, irow = 0);
			buttonsLayoutOnLandscape.Children.Add(btnSell,
				icol, ++irow);
			buttonsLayoutOnLandscape.Children.Add(btnDividend,
				icol, ++irow);
			buttonsLayoutOnLandscape.Children.Add(btnCost,
				icol, ++irow);
			buttonsLayoutOnLandscape.Children.Add(btnHistory,
				icol, ++irow);
			buttonsLayoutOnLandscape.Children.Add(btnOptions,
				icol, ++irow);

			Debug.Assert(irow + 1 == buttonsLayoutOnLandscape.RowDefinitions.Count);
		}


		private void OpenHistory(object sender, EventArgs ea)
		{
			//TODO
			var flows = model.Data.GetFlowEditor().GetFlowDetailsOrdered(new[] { "ASML" },
				new DateTime(2000, 1, 1), DateTime.Now
				).ToArray();
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
