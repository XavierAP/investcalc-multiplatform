using JP.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	internal sealed class PageMain : ContentPage, PortfolioView
	{
		private readonly ModelGateway model = new ModelGateway(GetDataFolder(), GetPriceAPILicense());

		readonly StackLayout flipLayout = new StackLayout();
		readonly StackLayout stocksLayout = new StackLayout();

		readonly (Label Tag, Label Value) averageReturn = (
			new Label { HorizontalTextAlignment = TextAlignment.End },
			new Label { HorizontalTextAlignment = TextAlignment.Start });

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

		static PageMain()
		{
			var layoutCol = new ColumnDefinition { Width = GridLength.Star };
			layoutCols = new ColumnDefinitionCollection { layoutCol, layoutCol, layoutCol, layoutCol };

			ExperimentalFeatures.Enable("ShareFileRequest_Experimental");
		}

		public PageMain()
		{
			this.Content = flipLayout;
			flipLayout.BackgroundColor = Format.BackgroundColor;

			var verticalLayout = new StackLayout { HorizontalOptions = layoutFillOption };
			verticalLayout.Children.Add(CreateAverageReturnView());
			verticalLayout.Children.Add(CreateStocksView());
			flipLayout.Children.Add(verticalLayout);
			
			PrepareLayouts();
			SizeChanged += OnSizeChanged;

			btnOptions.Clicked += PromptOptions;
			btnHistory.Clicked += async (s,e) => await Navigation.PushModalAsync(new PageHistory(
				model.Data.GetFlowEditor(),
				GetAllStockNames()));

			RefreshPortfolio();
		}

		private View CreateStocksView() => new ScrollView
		{
			Content = stocksLayout,
			HorizontalOptions = layoutFillOption,
			VerticalOptions   = layoutFillOption,
			Padding = 10,
		};

		private View CreateAverageReturnView()
		{
			var view = new Grid { ColumnDefinitions = layoutCols };
			view.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			const int row = 0;
			int col = 0;

			view.Children.Add(averageReturn.Tag,
				col, col+=2, row, row+1);

			view.Children.Add(averageReturn.Value,
				col, layoutCols.Count, row, row+1);

			return view;
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
				if(value == _PageOrientation) return;

				ResetLayout();
				if(value == Orientation.Landscape)
					SetLayoutLandscape();
				else
					SetLayoutPortrait();

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
			TryCalcReturnAvg();
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
				BackgroundColor = Format.FillColor,
				BorderColor = Format.LineColor,
				BorderWidth = 1,
				CornerRadius = 10,
			}, 0, icol = 3, irow, irow + 2);
			stockGrid.Children.Add(fields.TotalValue = new Label
			{
				Text = Format.ValueOnUnknownPrice(shares),
				HorizontalTextAlignment = TextAlignment.End,
				BackgroundColor = Format.FillColor,
			}, icol, irow);
			stockGrid.Children.Add(fields.Shares = new Label
			{
				Text = shares.FormatShares(),
				HorizontalTextAlignment = TextAlignment.End,
				BackgroundColor = Format.FillColor,
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
				fields.Return.Text = returnPer1Yearly.FormatPerCent();

			fields.Price.Completed += (s,e) => OnPriceChanged(name);

			stockIndex.Add(name, fields);
		}

		public void SetStockFigures(Stock stk, double? returnPer1Yearly)
		{
			var fields = stockIndex[stk.Name];
			fields.Shares.Text = stk.Shares.FormatShares();
			if(stk.Price.HasValue)
			{
				fields.Price.Text = stk.Price.ToString();
				fields.TotalValue.Text = (stk.Price * stk.Shares).FormatMoney();
			}
			else
			{
				fields.Price.Text = null;
				fields.TotalValue.Text = Format.ValueOnUnknownPrice(stk.Shares);
			}
			if(returnPer1Yearly.HasValue)
				fields.Return.Text = returnPer1Yearly.FormatPerCent();
			else
				fields.Return.Text = null;

			TryCalcReturnAvg();
		}

		
		private void PrepareLayouts()
		{
			// Portrait:
			buttonsLayoutOnPortrait.RowDefinitions.Add(layoutRowOther);

			// Landscape:
			buttonsLayoutOnLandscape.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			var layoutRow = new RowDefinition { Height = GridLength.Star };
			for(int i = 0; i < 3; ++i)
				buttonsLayoutOnLandscape.RowDefinitions.Add(layoutRow);
		}

		private void ResetLayout()
		{
			buttonsLayoutOnPortrait.Children.Clear();
			buttonsLayoutOnLandscape.Children.Clear();
			flipLayout.Children.Remove(buttonsLayoutOnPortrait);
			flipLayout.Children.Remove(buttonsLayoutOnLandscape);
		}

		private void SetLayoutPortrait()
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

		private void SetLayoutLandscape()
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

			flipLayout.Children.Add(buttonsLayoutOnLandscape);
			flipLayout.Orientation = StackOrientation.Horizontal;
		}


		private void OnPriceChanged(string stockName)
		{
			var fields = stockIndex[stockName];

			var stk = model.Portfolio.GetStock(stockName);
			if(double.TryParse(fields.Price.Text, out var price))
			{
				fields.TotalValue.Text = (price * stk.Shares).FormatMoney();
				fields.Return.Text = model.Calculator.CalcReturn(stk.Name, stk.Shares, price).FormatPerCent();
			}
			else
				fields.TotalValue.Text = Format.ValueOnUnknownPrice(stk.Shares);
			
			TryCalcReturnAvg();
		}

		private void TryCalcReturnAvg()
		{
			double total = 0;
			foreach(var fields in stockIndex.Values)
			{
				if(fields.TotalValue.Text == null)
				{
					averageReturn.Tag.Text =
					averageReturn.Value.Text = null;
					return;
				}
				total += double.Parse(fields.TotalValue.Text);
			}
			averageReturn.Tag.Text = "Average return:";
			averageReturn.Value.Text = model.Calculator.CalcReturnAvg(GetAllStockNames(), total).FormatPerCent();
		}

		private async void PromptStockActions(string stockName)
		{
			var option = await DisplayActionSheet(stockName, "Cancel", null,
				Operation.Buy.Text,
				Operation.Sell.Text,
				Operation.Dividend.Text,
				Operation.Cost.Text,
				"History",
				"Enter fetch code",
				"Edit stock name");
			switch(option)
			{
				case "History":
					await Navigation.PushModalAsync(new PageHistory(model.Data.GetFlowEditor(), stockName));
					break;

				case "Enter fetch code":
					string code = await DisplayPromptAsync(stockName, "Enter fetch code");
					if(null == code)
						break;
					model.Data.SetFetchCode(stockName, code);
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
				"Search fetch codes",
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

		private string[] GetAllStockNames() => stockIndex.Keys.ToArray();

		private Task DisplayError(Exception err) => DisplayAlert("ERROR! " + err.GetType().Name, err.Message, "OK");
	}
}
