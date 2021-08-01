using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JP.InvestCalc
{
	using Portfolio = SortedDictionary<string, Stock>;

	public class PortfolioData : IReadOnlyDictionary<string, Stock>
	{
		private readonly Portfolio stocks = new Portfolio(StringComparer.CurrentCultureIgnoreCase);
		private readonly Database database;
		private readonly PriceFetcher priceFetcher;
		private readonly ViewUpdater display;

		internal PortfolioData(ViewUpdater viewUpdater, Database database)
		{
			this.display = viewUpdater;
			this.database = database;
			this.priceFetcher = new PriceFetcher(this, display);
		}

		internal string ApiLicense
		{
			get => priceFetcher.ApiLicenseKey;
			set => priceFetcher.ApiLicenseKey = value;
		}

		public void Load(PortfolioView view)
		{
			var pricesKnown = (
				from stockByName in stocks
				let priceInfo = stockByName.Value.Price
				where priceInfo.HasValue
				select (Name: stockByName.Key, Price: priceInfo.Value)
				).ToDictionary(stk => stk.Name, stk => stk.Price);

			var namesToFetch = new List<string>(stocks.Count - pricesKnown.Count);

			stocks.Clear();
			var data = database.GetPortfolio();
			for(int i = 0; i < data.Count; i++)
			{
				var (name, shares) = data[i];
				Debug.Assert(shares >= 0, "Inconsistent database");

				var stk = new Stock(name, shares);
				stocks.Add(name, stk);
				display.AddStockToView(stk, view);

				if(pricesKnown.TryGetValue(name, out var price))
				{
					stk.Price = price;
					display.SetStockInView(stk, view);
				}
				else
					namesToFetch.Add(name);
			}
			FetchPrices(namesToFetch, view);
		}

		private void FetchPrices(IEnumerable<string> stocksToFetch, PortfolioView view)
		{
			priceFetcher.TryFetchPrices(database.GetFetchCodes(stocksToFetch), view);
		}
		private void FetchPrices(string nameToFetch, PortfolioView view)
			=> FetchPrices(new[] { nameToFetch }, view);

		public void AddShares(PortfolioView view, string stockName, double shares,
			double flow, DateTime date, string comment)
		{
			bool already = stocks.TryGetValue(stockName, out var stk);
			database.OpRecord(!already, stockName, date, shares, flow, comment);
			if(already)
			{
				stk.Shares += shares;
				display.SetStockInView(stk, view);
			}
			else
			{
				stk = new Stock(stockName, shares);
				stocks.Add(stockName, stk);
				display.AddStockToView(stk, view);
				FetchPrices(stockName, view);
			}
		}

		#region IReadOnlyDictionary implementation

		public IEnumerable<string> Keys => stocks.Keys;
		public IEnumerable<Stock> Values => stocks.Values;
		public int Count => stocks.Count;

		public Stock this[string key] => stocks[key];
		public bool ContainsKey(string key) => stocks.ContainsKey(key);
		public bool TryGetValue(string key, out Stock value) => stocks.TryGetValue(key, out value);

		IEnumerator<KeyValuePair<string, Stock>>
		IEnumerable<KeyValuePair<string, Stock>>.GetEnumerator() => stocks.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => stocks.GetEnumerator();

		#endregion
	}
}
