using System;

namespace JP.InvestCalc
{
	public class Stock
	{
		public string Name { get; }
		public double Shares { get; internal set; }
		public double? Price { get; internal set; }

		internal Stock(string name, double shares)
		{
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name),
				"Stock name must not be null or empty.");

			this.Name = name;
			this.Shares = shares;
		}

		public bool IsValueKnown(out double totalValue)
		{
			if(Shares == 0)
			{
				totalValue = 0;
				return true;
			}
			else if(Price.HasValue)
			{
				totalValue = Shares * Price.Value;
				return true;
			}
			else
			{
				totalValue = double.NaN;
				return false;
			}
		}
	}
}
