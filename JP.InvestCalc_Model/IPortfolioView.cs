using System;

namespace JP.InvestCalc
{
	public interface IPortfolioView
	{
		void InvokeOnUIThread(Action action);
		void AddStock(string name, double shares, double? returnPer1Yearly);
		void SetStockFigures(Stock stk, double? returnPer1Yearly);
	}
}
