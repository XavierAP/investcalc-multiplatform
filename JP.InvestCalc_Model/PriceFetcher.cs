using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JP.InvestCalc
{
	class PriceFetcher
	{
		public string ApiLicenseKey { get; set; }

		private Task fetching = Task.CompletedTask;

		private readonly PortfolioData portfolio;
		private readonly ViewUpdater display;

		internal PriceFetcher(PortfolioData portfolio, ViewUpdater display)
		{
			this.portfolio = portfolio;
			this.display = display;
		}

		public async void TryFetchPrices(
			IEnumerable<(string Name, string Code)> stocksToFetch,
			PortfolioView view)
		{
			if(string.IsNullOrWhiteSpace(ApiLicenseKey))
				return;
			if(!stocksToFetch.Any())
				return;

			await fetching;
			fetching = FetchPrices(stocksToFetch, view);
			await fetching;
		}

		private async Task FetchPrices(
			IEnumerable<(string Name, string Code)> stocksToFetch,
			PortfolioView view)
		{
			var quoter = new StockQuoter(ApiLicenseKey);
			var fetchJobs = (
				from s in stocksToFetch
				select FetchPrice(s, quoter)
				).ToList();

			while(fetchJobs.Any())
			{
				var done = await Task.WhenAny(fetchJobs);
				fetchJobs.Remove(done);
				if(done.IsFaulted) continue;

				var fetched = done.Result;
				if(fetched.IsFaulted) continue;

				var stk = portfolio.GetStock(fetched.StockName);
				stk.Price = fetched.Price;

				view?.InvokeOnUIThread(() => display.SetStockInView(stk, view));
			}
		}

		private async Task<(string StockName, double Price, bool IsFaulted)>
		FetchPrice((string Name, string Code) stock, StockQuoter quoter)
		{
			double price = double.NaN;
			bool isFaulted = false;
			try
			{
				price = await quoter.LoadPrice(stock.Code);
			}
			catch
			{
				isFaulted = true;
			}
			return (stock.Name, price, isFaulted);
		}
	}
}
