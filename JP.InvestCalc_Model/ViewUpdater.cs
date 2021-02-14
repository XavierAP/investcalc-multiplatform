namespace JP.InvestCalc
{
	class ViewUpdater
	{
		private readonly Calculator calculator;

		internal ViewUpdater(Calculator calculator) => this.calculator = calculator;

		public void AddStockToView(Stock stk, PortfolioView view)
		{
			view?.AddStock(stk.Name, stk.Shares, calculator.CalcReturn(stk));
		}

		public void SetStockInView(Stock stk, PortfolioView view)
		{
			view?.SetStockFigures(stk, calculator.CalcReturn(stk));
		}
	}
}
