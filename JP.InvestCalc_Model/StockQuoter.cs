using JP.Utils;
using System;
using System.Threading.Tasks;

namespace JP.InvestCalc
{
	class StockQuoter
	{
		internal StockQuoter(string licenseKey)
		{
			if(string.IsNullOrWhiteSpace(licenseKey)) throw new ArgumentNullException();

			license = licenseKey;
			retriever = new Retriever<double>();
			csvParser = new CsvParser();
		}
		
		/// <exception cref="Exception" />
		public async Task<double> LoadPrice(string code) => await retriever.Load(GetUrl(code), Parse);

		readonly string license;
		readonly Retriever<double> retriever;
		readonly CsvParser csvParser;

		private string GetUrl(string code)
		{
			if(string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException();
			return $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={code}&apikey={license}&datatype=csv";
		}

		private double Parse(string text)
		{
			var table = csvParser.Parse(text);

			if(table.RowCount() != 1) throw new FormatException(
				"Alpha Vantage web API bad CSV format:\n" +
				"expected two lines, headers and values.");

			var pos = table.Headers().IndexOf("price");
			return double.Parse(table.Row(1)[pos]);
		}
	}
}
