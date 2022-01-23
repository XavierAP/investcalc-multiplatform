using JP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	using Data = List<(long DatabaseId, DateTime Date, string StockName, double Shares, double Flow, double PriceAvg, string Comment)>;

	class PageHistory : TablePage
	{
		const ushort
			nColumns = 6,
			iColPrice = 4;

		readonly FlowEditor dataBase;
		CsvProcessor csv = new CsvProcessor(GetCsvSeparator());
		readonly string tempCsvFile = Path.Combine(Path.GetTempPath(), "InvestCalc-history.csv");

		readonly Data dataCache;
		
		readonly Button buttonDelete, buttonCsv;

		public bool HasChanged { get; private set; } = false;

		private bool IsEmpty => RowCount <= 1; // including headers

		public PageHistory(FlowEditor data, params string[] stockNames)
		{
			this.dataBase = data;

			AddHeaders("Date", "Stock", "Shares", "Flow", "Price", "Comment");
			AddData(dataCache = data.GetFlowDetailsOrdered(stockNames, DateTime.MinValue, DateTime.Now));

			buttonDelete = AddButtonAtBottom("Delete last", DoDelete);
			buttonDelete.IsEnabled = !IsEmpty;
			buttonCsv = AddButtonAtBottom("CSV", PromptCsvOptions);

			new OrientationFlipBehavior(this).SetOrChanged += OnOrientationSetOrChanged;

			Disappearing += (s,a) => File.Delete(tempCsvFile);
		}

		private async Task Close() => await Navigation.PopModalAsync();

		private void OnOrientationSetOrChanged(Orientation orientation)
		{
			buttonDelete.IsVisible = buttonCsv.IsVisible = orientation == Orientation.Portrait;
		}

		private void AddHeaders(params string[] headers)
		{
			AddRow();
			foreach(var h  in headers)
			{
				AddCellToCurrentRow(new Label
				{
					Text = h,
					FontAttributes = FontAttributes.Bold,
				});
			}
		}

		private void AddData(Data data)
		{
			for(int i = 0; i < data.Count; i++)
			{
				AddRow();
				foreach(var cell in FormatRow(data[i]))
					AddCellToCurrentRow(new Label { Text = cell });
			}
		}

		private static IEnumerable<string>
		FormatRow((long DatabaseId, DateTime Date, string StockName, double Shares, double Flow, double PriceAvg, string Comment) row)
		{
			yield return row.Date.ToShortDateString();
			yield return row.StockName;
			yield return row.Shares.FormatShares();
			yield return row.Flow.FormatMoneyPlusMinus();
			yield return row.PriceAvg.FormatMoneyPositive();
			yield return row.Comment;
		}
		
		#region Delete button

		private async void DoDelete(object sender, EventArgs args)
		{
			bool confirmed = await this.PromptConfirmation("Are you sure you want to delete the last record?");
			if(!confirmed) return;

			DeleteLastRecordFromDatabase();
			DeleteLastRow();
			if(IsEmpty) await Close();
		}

		private void DeleteLastRecordFromDatabase()
		{
			var lastIndex = dataCache.Count - 1;
			DeleteLastCache[0] = dataCache[lastIndex].DatabaseId;
			dataCache.RemoveAt(lastIndex);
			dataBase.DeleteFlows(DeleteLastCache);
			HasChanged = true;
		}
		readonly long[] DeleteLastCache = new long[1];

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

				case "Configure":
					await this.PromptAndAct(SetCsvSeparator, "CSV", "Enter separator:");
					break;

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
			var rowCache = new List<string>(nColumns - 1);
			for(int r = 0; r < dataCache.Count; r++, rowCache.Clear())
			{
				rowCache.AddRange(FormatRow(dataCache[r]).Where(
					(cell, index) => iColPrice != index )); // skip dependent variable
				
				yield return rowCache;
			}
		}

		private async Task ImportCsvFile()
		{
			var file = await FilePicker.PickAsync();
			if(file == null) return;

			int n;
			try {
				n = dataBase.ImportFlows(File.ReadAllText(file.FullPath), csv);
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

		private static string GetCsvSeparator()
		{
			if(Application.Current.Properties.TryGetValue("CsvSeparator", out object sep))
				return sep as string;
			else
				return Config.DefaultCsvSeparator;
		}
		private async Task SetCsvSeparator(string separator)
		{
			if(0 == separator.Length)
			{
				await DisplayAlert(null, "Separator can't be empty.", "OK");
				return;
			}
			this.csv = new CsvProcessor(separator);
			Application.Current.Properties["CsvSeparator"] = separator;
			await Application.Current.SavePropertiesAsync();
		}

		#endregion
	}
}
