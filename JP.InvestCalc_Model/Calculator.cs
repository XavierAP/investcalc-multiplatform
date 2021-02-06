using JP.Maths;
using System;

namespace JP.InvestCalc
{
	public class Calculator
	{
		private readonly Database database;

		internal Calculator(Database database) => this.database = database;

		public double CalcReturn(string name, double shares, double price)
		{
			return Money.SolveRateInvest(database.GetFlows(name),
				(shares * price, DateTime.Now.Date), Config.PrecisionPerCent + 2);
		}

		internal double? CalcReturn(Stock stk)
		{
			if(stk.Price.HasValue)
				return CalcReturn(stk.Name, stk.Shares, stk.Price.Value);
			else if(stk.Shares == 0)
				return CalcReturn(stk.Name, 0, 0);
			else
				return null;
		}

		public double CalcReturnAvg(string[] stockNames, double totalValue)
		{
			return Money.SolveRateInvest(database.GetFlows(stockNames),
				(totalValue, DateTime.Today.Date), Config.PrecisionPerCent + 2);
		}
	}
}
