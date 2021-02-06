using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JP.InvestCalc
{
	class PriceFetcher
	{
		private Task fetching = Task.CompletedTask;

		private readonly string apiLicense;
		private readonly PortfolioData portfolio;
		private readonly ViewUpdater display;

		public PriceFetcher(string apiLicense, PortfolioData portfolio, ViewUpdater display)
		{
			this.portfolio = portfolio;
			this.apiLicense = apiLicense;
			this.display = display;
		}

		public async void TryFetchPrices(
			IEnumerable<(string Name, string Code)> stocksToFetch,
			IPortfolioView view)
		{
			if(!stocksToFetch.Any()) return;
			await fetching;
			fetching = FetchPrices(stocksToFetch, view);
			await fetching;
		}

		private async Task FetchPrices(
			IEnumerable<(string Name, string Code)> stocksToFetch,
			IPortfolioView view)
		{
			var fetchJobs = (
				from s in stocksToFetch
				select FetchPrice(s)
				).ToList();

			while(fetchJobs.Any())
			{
				var done = await Task.WhenAny(fetchJobs);
				fetchJobs.Remove(done);
				if(done.IsFaulted) continue;

				var fetched = done.Result;
				if(fetched.Error != null) continue;

				var stk = portfolio.GetStock(fetched.StockName);
				stk.Price = fetched.Price;

				view?.InvokeOnUIThread(() => display.SetStockInView(stk, view));
			}
		}

		private async Task<(string StockName, double Price, Exception Error)>
		FetchPrice((string Name, string Code) stock)
		{
			var qt = Quote.Prepare(stock.Code, apiLicense);
			if(qt == null)
				return (stock.Name, 0, new Exception(
					$"Invalid fetch code \"{stock.Code}\"."));

			var price = await qt.LoadPrice();
			return (stock.Name, price, qt.Error);
		}
	}
}
