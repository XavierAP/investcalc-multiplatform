﻿using JP.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	internal sealed class PageMain : ContentPage, PortfolioView
	{
		readonly ModelGateway model = new ModelGateway(
			Config.GetDataFolder(), GetPriceAPILicense());

		readonly StackLayout flipLayout = new StackLayout();
		readonly StackLayout stocksLayout = new StackLayout();
		
		readonly (Label Tag, Label Value) totalValue = (
			new Label { HorizontalTextAlignment = TextAlignment.End }.SetFontSizeMediumSmall(),
			new Label { HorizontalTextAlignment = TextAlignment.Start }.SetFontSizeMediumSmall());
		readonly (Label Tag, Label Value) averageReturn = (
			new Label { HorizontalTextAlignment = TextAlignment.End }.SetFontSizeMediumSmall(),
			new Label { HorizontalTextAlignment = TextAlignment.Start }.SetFontSizeMediumSmall());

		readonly Grid buttonsLayoutOnPortrait;
		readonly Grid buttonsLayoutOnLandscape;

		readonly Button
			btnBuy      = new Button(),
			btnHistory  = new Button { Text = "History" },
			btnOptions  = new Button { Text = "Options" };

		readonly Dictionary<string, (Label Shares, Label Price, Label Total, Label NetGain, Label GainRatio, Label Yearly)>
		stockIndex = new Dictionary<string, (Label Shares, Label Price, Label Total, Label NetGain, Label GainRatio, Label Yearly)>();
		
		readonly RowDefinition layoutRowHeader = new RowDefinition { Height = GridLength.Auto };
		readonly RowDefinition layoutRowOther = new RowDefinition { Height = GridLength.Auto };
		readonly ColumnDefinitionCollection layoutCols;

		public PageMain()
		{
			ExperimentalFeatures.Enable("ShareFileRequest_Experimental");

			ConstructLayouts(out layoutCols, out buttonsLayoutOnPortrait, out buttonsLayoutOnLandscape);

			this.Content = flipLayout;
			flipLayout.BackgroundColor = Format.BackgroundColor;

			var verticalLayout = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };
			verticalLayout.Children.Add(CreateAggregateView());
			verticalLayout.Children.Add(CreateStocksView());
			flipLayout.Children.Add(verticalLayout);

			new OrientationFlipBehavior(this).SetOrChanged += OnOrientationSetOrChanged;
			PrepareLayouts();
			SetButtonEvents();
			RefreshPortfolio();
		}

		static void ConstructLayouts(
			out ColumnDefinitionCollection layoutCols,
			out Grid buttonsLayoutOnPortrait,
			out Grid buttonsLayoutOnLandscape)
		{
			var layoutCol = new ColumnDefinition { Width = GridLength.Star };
			layoutCols = new ColumnDefinitionCollection { layoutCol, layoutCol, layoutCol, layoutCol };

			buttonsLayoutOnPortrait = new Grid
			{
				ColumnDefinitions = layoutCols,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions   = LayoutOptions.End,
			};
			buttonsLayoutOnLandscape = new Grid
			{
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions   = LayoutOptions.FillAndExpand,
			};
		}

		void SetButtonEvents()
		{
			btnOptions.Clicked += PromptOptions;

			btnHistory.Clicked += async (s, e) => await DisplayHistory(GetAllStockNames());

			btnBuy.Clicked += async (s,e) => await Navigation.PushModalAsync(
				PageOperation.BuyNewStock(model.Portfolio, this));
		}

		View CreateStocksView() => new ScrollView
		{
			Content = stocksLayout,
			HorizontalOptions = LayoutOptions.FillAndExpand,
			VerticalOptions   = LayoutOptions.FillAndExpand,
			Padding = 10,
		};

		View CreateAggregateView()
		{
			var view = new Grid { ColumnDefinitions = layoutCols };
			view.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			const int row = 0;
			int col = 0;
			
			view.Children.Add(totalValue.Tag,      col, ++col, row, row+1);
			view.Children.Add(totalValue.Value,    col, ++col, row, row+1);
			view.Children.Add(averageReturn.Tag,   col, ++col, row, row+1);
			view.Children.Add(averageReturn.Value, col, ++col, row, row+1);

			return view;
		}

		
		void OnOrientationSetOrChanged(Orientation orientation)
		{
			ResetLayout();
			if(Orientation.Landscape == orientation)
				SetLayoutLandscape();
			else
				SetLayoutPortrait();
		}


		void RefreshPortfolio()
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
			TryCalcReturnAvg();
		}

		void PortfolioView.InvokeOnUIThread(Action action) => MainThread.BeginInvokeOnMainThread(action);

		void PortfolioView.AddStock(string name, double shares,
			(double NetGain, double GainRatio, double YearlyPer1)? info)
		{
			var stockGrid = new Grid { ColumnDefinitions = layoutCols };
			stocksLayout.Children.Add(stockGrid);
			(Label Shares, Label Price, Label Total, Label NetGain, Label GainRatio, Label Yearly) fields;
			Button btn;

			int irow = 0;

			stockGrid.RowDefinitions.Add(layoutRowHeader);
			stockGrid.Children.Add(btn = new Button
			{
				Text = name,
				FontAttributes = FontAttributes.Bold,
				BackgroundColor = Format.FillColor,
				BorderColor = Format.LineColor,
				BorderWidth = 1,
				CornerRadius = 10,
			}, 0, 4, irow, ++irow);
			btn.Clicked += (s, e) => PromptStockActions(name);

			const int
				left = 0,
				right = left + 2;
			
			stockGrid.RowDefinitions.Add(layoutRowOther);
			fields.Shares    = AddCell("Shares:", stockGrid, left , irow);
			fields.NetGain   = AddCell("Gain:"  , stockGrid, right, irow);
			stockGrid.RowDefinitions.Add(layoutRowOther);
			fields.Price     = AddCell("Price:" , stockGrid, left , ++irow);
			fields.GainRatio = AddCell("Gain/Invest:", stockGrid, right, irow);
			stockGrid.RowDefinitions.Add(layoutRowOther);
			fields.Total     = AddCell("Value:" , stockGrid, left , ++irow);
			fields.Yearly    = AddCell("Yearly:", stockGrid, right, irow);

			fields.Shares.Text = shares.FormatShares();
			if(info.HasValue)
			{
				fields.NetGain.Text = info.Value.NetGain.FormatMoneyPlusMinus();
				fields.GainRatio.Text = info.Value.GainRatio.FormatPerCent();
				fields.Yearly.Text = info.Value.YearlyPer1.FormatPerCent();
			}
			stockIndex.Add(name, fields);
		}

		void PortfolioView.SetStockFigures(Stock stk,
			(double NetGain, double GainRatio, double YearlyPer1)? info)
		{
			var fields = stockIndex[stk.Name];
			fields.Shares.Text = stk.Shares.FormatShares();
			if(stk.Price.HasValue)
			{
				fields.Price.Text = stk.Price.Value.FormatMoneyPositive();
				fields.Total.Text = (stk.Price * stk.Shares).Value.FormatMoneyPositive();
			}
			else
			{
				fields.Price.Text =
				fields.Total.Text = null;
			}
			if(info.HasValue)
			{
				fields.NetGain.Text = info.Value.NetGain.FormatMoneyPlusMinus();
				fields.GainRatio.Text = info.Value.GainRatio.FormatPerCent();
				fields.Yearly.Text = info.Value.YearlyPer1.FormatPerCent();
			}
			else
			{
				fields.NetGain.Text =
				fields.GainRatio.Text =
				fields.Yearly.Text = null;
			}
			TryCalcReturnAvg();
		}

		static Label AddCell(string caption, Grid stockGrid, int icol, int irow)
		{
			stockGrid.Children.Add(new Label
			{
				Text = caption,
				HorizontalTextAlignment = TextAlignment.End,
			}.SetFontSizeMediumSmall(), icol, irow);

			var ans = new Label().SetFontSizeMediumSmall();
			stockGrid.Children.Add(ans, icol + 1, irow);

			return ans;
		}

		
		void PrepareLayouts()
		{
			// Portrait:
			buttonsLayoutOnPortrait.RowDefinitions.Add(layoutRowOther);

			// Landscape:
			buttonsLayoutOnLandscape.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			var layoutRow = new RowDefinition { Height = GridLength.Star };
			for(int i = 0; i < 6; ++i)
				buttonsLayoutOnLandscape.RowDefinitions.Add(layoutRow);
		}

		void ResetLayout()
		{
			buttonsLayoutOnPortrait.Children.Clear();
			buttonsLayoutOnLandscape.Children.Clear();
			flipLayout.Children.Remove(buttonsLayoutOnPortrait);
			flipLayout.Children.Remove(buttonsLayoutOnLandscape);
		}

		void SetLayoutPortrait()
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

			flipLayout.Children.Add(buttonsLayoutOnPortrait);
			flipLayout.Orientation = StackOrientation.Vertical;
		}

		void SetLayoutLandscape()
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

			flipLayout.Children.Add(buttonsLayoutOnLandscape);
			flipLayout.Orientation = StackOrientation.Horizontal;
		}


		async Task TrySetPrice(string stockName, string priceInput)
		{
			bool isInputValid = double.TryParse(priceInput, out var price);
			if(!isInputValid || price < 0)
			{
				await DisplayAlert(null, "Invalid input", "OK");
				return;
			}
			var stk = model.Portfolio[stockName];
			var total = price * stk.Shares;

			var fields = stockIndex[stockName];
			fields.Price.Text = price.FormatMoneyPositive();
			fields.Total.Text = total.FormatMoneyPositive();

			var info = model.Calculator.CalcReturn(stk.Name, total);
			fields.NetGain.Text = info.NetGain.FormatMoneyPlusMinus();
			fields.GainRatio.Text = info.GainRatio.FormatPerCent();
			fields.Yearly.Text = info.YearlyPer1.FormatPerCent();
			
			TryCalcReturnAvg();
		}

		void TryCalcReturnAvg()
		{
			double total = 0;
			foreach(var stk in model.Portfolio.Values)
			{
				if(stk.IsValueKnown(out var value))
					total += value;
				else
				{
					averageReturn.Tag.Text =
					averageReturn.Value.Text = null;
					return;
				}
			}
			totalValue.Tag.Text = "Total value:";
			totalValue.Value.Text = total.FormatMoneyPositive();
			averageReturn.Tag.Text = "Avg. return:";
			averageReturn.Value.Text = model.Calculator.CalcReturnAvg(GetAllStockNames(), total).FormatPerCent();
		}

		async void PromptStockActions(string stockName)
		{
			var option = await DisplayActionSheet(stockName, "Cancel", null,
				Operation.Buy.Text,
				Operation.Sell.Text,
				Operation.Dividend.Text,
				Operation.Cost.Text,
				"History",
				"- Enter fetch code",
				"- or enter price manually",
				"Edit stock name");

			if(option == "History")
			{
				await DisplayHistory(stockName);
			}
			else if(option =="- Enter fetch code")
			{
				await this.PromptAndAct(SetFetchCode, stockName, "Enter fetch code");
				async Task SetFetchCode(string code)
				{
					model.Data.SetFetchCode(stockName, code);
					RefreshPortfolio();
				}
			}
			else if(option == "- or enter price manually")
			{
				await this.PromptAndAct(SetPrice, stockName, "Enter price", keyboard: Keyboard.Numeric);
				async Task SetPrice(string price) => await TrySetPrice(stockName, price);
			}
			else if(option == "Edit stock name")
			{
				await this.PromptAndAct(SetStockName, stockName, "Enter a new name", initialValue: stockName);
				async Task SetStockName(string newName)
				{
					if(string.IsNullOrWhiteSpace(newName)) return;
					model.Data.SetStockName(stockName, newName);
					RefreshPortfolio();
				}
			}
			else
			{
				var operation = Operation.All.SingleOrDefault(op => op.Text == option);
				if(operation != null)
					await Navigation.PushModalAsync(
						PageOperation.OnExistingStock(operation, stockName, model.Portfolio, this));
			}
		}

		async Task DisplayHistory(params string[] stockNames)
		{
			var history = new PageHistory(model.Data.GetFlowEditor(), stockNames);
			history.Disappearing += (s,e) =>
			{
				if(history.HasChanged)
					RefreshPortfolio();
			};
			await Navigation.PushModalAsync(history);
		}

		async void PromptOptions(object sender, EventArgs ea)
		{
			var option = await DisplayActionSheet("Options", "Cancel", null,
				"Export data file",
				"Import data file",
				"Search fetch codes",
				"Price lookup license");
			switch(option)
			{
				case "Export data file": await ExportDataFile(); break;
				case "Import data file": await ImportDataFile(); break;

				case "Search fetch codes": await SearchStocks(); break;

				case "Price lookup license":
					await this.PromptAndAct(SetPriceAPILicense, "API license",
						"Enter your key from AlphaVantage.co:", initialValue: model.ApiLicenseKey);
					break;

				case "Cancel":
				default:
					return;
			}
		}

		Task ExportDataFile() => Share.RequestAsync(new ShareFileRequest
		{
			File = new ShareFile(model.Data.FilePath, "application/octet-stream")
		});

		async Task ImportDataFile()
		{
			var file = await FilePicker.PickAsync();
			if(file == null) return;

			var err = FileBackup.TryCopy(file.FullPath, model.Data.FilePath);
			if(err == null)
				RefreshPortfolio();
			else
				await this.DisplayError(err);
		}
		

		async Task SearchStocks()
		{
			await this.PromptAndAct(ShowSearchResults, "Search stocks", "Enter search term:");
			async Task ShowSearchResults(string keyword)
			{
				if(string.IsNullOrWhiteSpace(keyword)) return;
				var results = await new StockSearcher().Search(keyword, model.ApiLicenseKey);
				if(results.Length == 0)
				{
					await DisplayAlert(null, "No results", "OK");
					return;
				}
				await Navigation.PushModalAsync(new PageSearch(results));
			};
		}


		async Task SetPriceAPILicense(string license)
		{
			File.WriteAllText(GetAPILicenseFileName(), license);
			model.ApiLicenseKey = license;
			RefreshPortfolio();
		}

		static string GetPriceAPILicense()
		{
			var pathName = GetAPILicenseFileName();
			if(!File.Exists(pathName)) return null;

			return File.ReadAllText(pathName).Trim();
		}

		static string GetAPILicenseFileName() => Path.Combine(
			Config.GetDataFolder(), "api-license.txt");

		string[] GetAllStockNames() => stockIndex.Keys.ToArray();
	}
}
