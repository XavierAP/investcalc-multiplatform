namespace JP.InvestCalc
{
	class ViewUpdater
	{
		private readonly Calculator calculator;

		public ViewUpdater(Calculator calculator) => this.calculator = calculator;

		public void AddStockToView(Stock stk, IPortfolioView view)
		{
			view?.AddStock(stk.Name, stk.Shares, calculator.CalcReturn(stk));
		}

		public void SetStockInView(Stock stk, IPortfolioView view)
		{
			view?.SetStockFigures(stk, calculator.CalcReturn(stk));
		}
	}
}
