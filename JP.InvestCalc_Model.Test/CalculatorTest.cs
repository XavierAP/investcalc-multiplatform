namespace JP.InvestCalc
{
	[TestFixture(TestOf = typeof(Calculator))]
	class CalculatorTest
	{
		const double tolerance = 1e-9;

		[TestCase(1_001.00, new double[] { -1_000 }, 1.00, 0.001)]
		[TestCase(   50.00, new double[] { -100, 25 }, -25.00, -0.25)]

		public void CalculateGain(double presentValue, IEnumerable<double> cashFlows,
			double expectedGain, double expectedRatio)
		{
			var result = Calculator.CalculateGain(presentValue, cashFlows);

			Assert.That(result.NetGain,   Is.EqualTo(expectedGain ).Within(tolerance));
			Assert.That(result.GainRatio, Is.EqualTo(expectedRatio).Within(tolerance));
		}
	}
}
