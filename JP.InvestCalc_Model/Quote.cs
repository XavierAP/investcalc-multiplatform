using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JP.InvestCalc
{
	/// <summary>Quote of a stock's price
	/// consulted asynchronously.</summary>
	abstract class Quote
	{
		/// <summary>Stock identifier.</summary>
		public string Code { get; private set; }

		/// <summary>Returns nonsense value in case of network error (see <see cref="Error"/>).</returns>
		public async Task<double> LoadPrice()
		{
			try
			{
				var response = await new HttpClient().GetAsync(Url);
				response.EnsureSuccessStatusCode();
				var data = await response.Content.ReadAsStringAsync();

				return ParsePrice(data); // may throw
			}
			catch(Exception err)
			{
				Error = err;
				return ErrorPrice;
			}
		}

		/// <summary>Null if <see cref="LoadPrice"/> was successful
		/// -- or not called yet.</summary>
		public Exception Error { get; private set; }

		protected const double ErrorPrice = double.NaN;

		protected abstract string Url { get; }
		protected abstract double ParsePrice(string data);

		private readonly static char[]
		separator = " \t\r\n".ToCharArray();

		protected Quote(string code)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(code));
			Code = code;
		}
	}

	/// <summary><see cref="Quote"/> acquired from AlphaVantage.co</summary>
	class QuoteAlphaVantage : Quote
	{
		public QuoteAlphaVantage(string code, string license) : base(code)
		{
			if(string.IsNullOrWhiteSpace(license))
				throw new ArgumentException("API key required from AlphaVantage.co.");

			this.LicenseKey = license;
		}

		private readonly string LicenseKey;

		/// <summary>CSV retrieved from the web API
		/// after <see cref="Quote.LoadPrice"/> has been called
		/// -- null beforehand.</summary>
		public string DataCSV { get; private set; }

		protected override string Url => $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={Code}&apikey={LicenseKey}&datatype=csv";

		protected override double ParsePrice(string data)
		{
			DataCSV = data;

			var cross_pairs = data.Split(separatorLine, StringSplitOptions.RemoveEmptyEntries)
				.Select(lin => lin.Split(separatorCSV, StringSplitOptions.None))
				.ToArray();

			if(cross_pairs.Length != 2) throw new IOException(
				"Alpha Vantage web API bad CSV format:\n" +
				"expected two lines, headers and values.");

			string[]
				tags = cross_pairs[0],
				vals = cross_pairs[1];

			Debug.Assert(tags.Length == vals.Length, "Web API error");
			Debug.Assert(
				0 == string.Compare("symbol", tags[0], false) &&
				0 == string.Compare( Code   , vals[0], true ) ,
				"Alpha Vantage web API warning.");

			const string tagPrice = "price";
			var pos = Array.IndexOf(tags, tagPrice);
			if(pos < 0 || pos >= vals.Length) throw new IOException(
				"Alpha Vantage web API bad CSV format:\n" +
				$"cannot find value under header {tagPrice}.");

			return double.Parse(vals[pos]);
		}
		private readonly static char[]
			separatorLine = "\r\n".ToCharArray(),
			separatorCSV  = ",".ToCharArray();
	}
}
