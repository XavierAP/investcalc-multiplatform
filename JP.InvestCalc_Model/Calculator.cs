using JP.Maths;
using System;
using System.Collections.Generic;

namespace JP.InvestCalc
{
	public class Calculator
	{
		const byte PrecisionPer1 = 4;

		private readonly Database database;

		internal Calculator(Database database) => this.database = database;

		public (double NetGain, double YearlyPer1)
		CalcReturn(string name, double shares, double price)
		{
			var flows = database.GetFlows(name);
			var currentValue = shares * price;
			var netGain = AddUpCash(flows) + currentValue;
			var yearly = Money.SolveRateInvest(flows,
				(currentValue, DateTime.Now.Date), PrecisionPer1);
			return (netGain, yearly);
		}

		internal (double NetGain, double YearlyPer1)?
		CalcReturn(Stock stk)
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
				(totalValue, DateTime.Today.Date), PrecisionPer1);
		}

		private double AddUpCash(List<(double Cash, DateTime Day)> flows)
		{
			double total = 0;
			for(int i = 0; i < flows.Count; i++)
				total += flows[i].Cash;

			return total;
		}
	}
}
