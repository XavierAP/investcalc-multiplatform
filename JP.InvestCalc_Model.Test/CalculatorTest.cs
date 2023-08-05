namespace JP.InvestCalc
{
	[TestFixture(TestOf = typeof(Calculator))]
	class CalculatorTest
	{
		const double tolerance = 1e-9;

		[TestCase(1_001.00, new double[] { -1_000 }, 1.00, 0.001)]
		[TestCase(   50.00, new double[] { -100, 25 }, -25.00, -0.25)]
		[TestCase(   15.00, new double[] { -10, 1, -10 }, -4.00, -0.20)]
		[TestCase(   14.00, new double[] { -8, 1, -4, 2 }, 5.00, 5.00/12)]

		public void CalculateGain(double presentValue, IEnumerable<double> cashFlows,
			double expectedGain, double expectedRatio)
		{
			var result = Calculator.CalculateGain(presentValue, cashFlows);

			Assert.That(result.NetGain,   Is.EqualTo(expectedGain ).Within(tolerance));
			Assert.That(result.GainRatio, Is.EqualTo(expectedRatio).Within(tolerance));
		}
	}
}
