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

		public (double NetGain, double GainRatio, double YearlyPer1)
		CalcReturn(string name, double totalValue)
		{
			var flows = database.GetFlows(name);
			var (netGain, gainRatio) = CalculateGain(flows,  totalValue);
			var yearly = Money.SolveRateInvest(flows, (totalValue, DateTime.Now.Date), PrecisionPer1);
			return (netGain, gainRatio, yearly);
		}

		internal (double NetGain, double GainToInvest, double YearlyPer1)?
		CalcReturn(Stock stk)
		{
			if(stk.IsValueKnown(out var totalValue))
				return CalcReturn(stk.Name, totalValue);
			else
				return null;
		}

		public double CalcReturnAvg(string[] stockNames, double totalValue)
		{
			return Money.SolveRateInvest(database.GetFlows(stockNames),
				(totalValue, DateTime.Today.Date), PrecisionPer1);
		}

		private (double NetGain, double GainRatio)
		CalculateGain(List<(double Cash, DateTime Day)> flows, double presentValue)
		{
			double
				net = 0,
				min = 0;

			for(int i = 0; i < flows.Count; i++)
			{
				net += flows[i].Cash;
				if(net < min) min = net;
			}
			net += presentValue;
			return (net, - net / min);
		}
	}
}
