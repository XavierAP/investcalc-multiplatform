using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace JP.InvestCalc
{
	using Portfolio = SortedDictionary<string, Stock>;

	public class PortfolioData
	{
		private readonly Portfolio stocks = new Portfolio(StringComparer.CurrentCultureIgnoreCase);
		private readonly Database database;
		private readonly PriceFetcher priceFetcher;
		private readonly ViewUpdater display;

		internal PortfolioData(ViewUpdater viewUpdater, Database database, string priceApiLicenseKey)
		{
			this.display = viewUpdater;
			this.database = database;
			this.priceFetcher = new PriceFetcher(priceApiLicenseKey, this, display);
		}

		public Stock GetStock(string name) => stocks[name];

		public string PriceApiLicenseKey
		{
			set => priceFetcher.ApiLicenseKey = value;
		}

		public void Load(IPortfolioView view)
		{
			var pricesKnown = (
				from stockByName in stocks
				let priceInfo = stockByName.Value.Price
				where priceInfo.HasValue
				select (Name: stockByName.Key, Price: priceInfo.Value)
				).ToDictionary(stk => stk.Name, stk => stk.Price);

			var namesToFetch = new List<string>(stocks.Count - pricesKnown.Count);

			stocks.Clear();
			foreach(var (name, shares) in database.GetPortfolio())
			{
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

		private void FetchPrices(IEnumerable<string> stocksToFetch, IPortfolioView view)
		{
			if(stocksToFetch.Any())
				priceFetcher?.TryFetchPrices(database.GetFetchCodes(stocksToFetch), view);
		}
		private void FetchPrices(string nameToFetch, IPortfolioView view)
			=> FetchPrices(new[] { nameToFetch }, view);

		public void AddShares(IPortfolioView view, string stockName, double shares,
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
	}
}
