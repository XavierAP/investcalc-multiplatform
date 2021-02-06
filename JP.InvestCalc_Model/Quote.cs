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
		/// <summary>Each value maps to an implementation of <see cref="Quote"/>
		/// and can be passed to <see cref="Quote.Prepare(Provider, string)"/>.</summary>
		public enum Provider
		{
			AlphaVantage
		}

		/// <summary>Stock identifier.</summary>
		public string Code { get; private set; }

		/// <summary>Gets the price from the Internet.</summary>
		/// <returns>Nonsense value in case of network error (see <see cref="Error"/>).</returns>
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

		/// <summary>Factory method.</summary>
		/// <param name="api">Website/API where the quote will be got from,
		/// which maps to <see cref="Provider"/>, e.g. "AlphaVantage".</param>
		/// <param name="code">code of the stock on this website, e.g. "ASML.AMS"</param>
		/// <returns>Null if code is invalid.
		/// Otherwise object that provides the quote from the appropriate website.</returns>
		public static Quote Prepare(Provider api, string code, string license)
		{
			if(string.IsNullOrWhiteSpace(code)) return null;

			switch(api)
			{
				case Provider.AlphaVantage:
					return new QuoteAlphaVantage(code, license);
				
				default: return null;
			}
		}

		/// <summary>Factory method.</summary>
		/// <param name="provider_code">Two "words" separated by a space.
		/// The first determines the website where the quote will be got from, e.g. "AlphaVantage".
		/// The second word is the code of the stock on this website, e.g. "ASML.AMS".</param>
		/// <returns>Null if code is invalid.
		/// Otherwise object that provides the quote from the appropriate website.</returns>
		public static Quote Prepare(string provider_code, string license)
		{
			if(provider_code == null) return null;

			const int n = 2; // words needed; any more are ignored
			string[] words = provider_code.Split(separator, StringSplitOptions.RemoveEmptyEntries);

			if(words.Length < n || words.Take(n).Any(c => string.IsNullOrWhiteSpace(c)))
				return null;

			string apiName = words[0].Trim();
			apiName = TranslateApiNameIfNeeded(apiName);
			string code = words[1].Trim();

			foreach(Provider api in Enum.GetValues(typeof(Provider)))
				if(0 == string.Compare(apiName, api.ToString(), true))
					return Prepare(api, code, license);
			
			// else:
			return null;
		}

		private static string TranslateApiNameIfNeeded(string fromUser)
		{
			switch(fromUser.ToUpper())
			{
				case "AV": return Provider.AlphaVantage.ToString();
				default: return fromUser;
			}
		}

		private readonly static char[]
		separator = " \t\r\n".ToCharArray();

		/// <summary>Constructor</summary>
		/// <param name="code">Stock identifier.</param>
		protected Quote(string code)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(code));
			Code = code;
		}
	}

	/// <summary><see cref="Quote"/> acquired from AlphaVantage.co</summary>
	class QuoteAlphaVantage : Quote
	{
		/// <summary>Constructor</summary>
		/// <param name="code">Stock identifier.</param>
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
