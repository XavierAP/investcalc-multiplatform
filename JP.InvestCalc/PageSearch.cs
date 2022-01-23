using JP.Utils;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	class PageSearch : TablePage
	{
		public PageSearch(string[][] results)
		{
			for(int r = 0; r < results.Length; r++)
			{
				var row = results[r];
				AddRow();

				var stockCode = row[0];
				var button = new Button { Text = stockCode };
				button.Clicked += async (s,a) => await Click(stockCode);
				AddCellToCurrentRow(button);

				for(int c = 1; c < row.Length; c++)
					AddCellToCurrentRow(new Label
					{
						Text = row[c],
						VerticalTextAlignment = TextAlignment.Center,
					});
			}
		}

		private async Task Click(string stockCode)
		{
			await Clipboard.SetTextAsync(stockCode);
			await DisplayAlert(stockCode, "Fetch code copied to clipboard.", "OK");
		}
	}
}
