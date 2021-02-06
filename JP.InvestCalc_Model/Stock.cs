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
	}
}
