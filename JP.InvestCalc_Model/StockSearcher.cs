using JP.Utils;
using System;
using System.Threading.Tasks;

namespace JP.InvestCalc
{
	public class StockSearcher
	{
		readonly Retriever retriever;
		readonly CsvParser csvParser;

		public StockSearcher()
		{
			retriever = new Retriever();
			csvParser = new CsvParser();
		}
		
		/// <summary>Returns for each row: 0) fetchCode, 1) name in API, 2) currency, 3) region, 4) type.</summary>
		/// <exception cref="Exception" />
		public async Task<string[][]> Search(string keywords, string apiLicenseKey) => (
			Parse(await retriever.Load(GetUrl(keywords, apiLicenseKey)))
			).Filter("symbol", "name", "currency", "region", "type");

		private string GetUrl(string keywords, string apiLicenseKey)
		{
			if(string.IsNullOrWhiteSpace(apiLicenseKey) || string.IsNullOrWhiteSpace(keywords)) throw new ArgumentNullException();
			return $"https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={keywords}&apikey={apiLicenseKey}&datatype=csv";
		}

		private string[][] Parse(string text) => csvParser.Parse(text);
	}
}
