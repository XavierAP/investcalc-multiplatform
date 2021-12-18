using System;

namespace JP.InvestCalc
{
	public interface PortfolioView
	{
		void InvokeOnUIThread(Action action);

		void AddStock(string name, double shares,
			(double NetGain, double YearlyPer1)? info);

		void SetStockFigures(Stock stk,
			(double NetGain, double YearlyPer1)? info);
	}
}
