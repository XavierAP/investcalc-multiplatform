using JP.Maths;
using JP.Maths.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

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
			var (netGain, gainRatio) = CalculateGain(totalValue, flows);
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

		public static (double NetGain, double GainRatio) CalculateGain(
			double presentValue,
			IEnumerable<(double Cash, DateTime Day)> flows)
		{
			return CalculateGain(presentValue, from f in flows select f.Cash);
		}

		public static (double NetGain, double GainRatio) CalculateGain(
			double presentValue,
			IEnumerable<double> cashFlows)
		{
			var stats = new Maths.Statistics.BatchAggregator();
			var net = stats.Add<Sum>();
			var min = stats.Add<Min>();

			foreach (var flow in cashFlows)
			{
				stats.Aggregate(flow);
			}
			var minResult = min.GetResult();
			stats.Aggregate(presentValue);
			var netResult = net.GetResult();
			
			return (netResult, - netResult / minResult);
		}
	}
}
