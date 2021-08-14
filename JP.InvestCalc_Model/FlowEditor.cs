using System;
using System.Collections.Generic;

namespace JP.InvestCalc
{
	public class FlowEditor
	{
		private readonly Database database;

		internal FlowEditor(Database database) => this.database = database;

		public bool IsDataChanged => database.IsPortfolioChanged;

		public List<(long DatabaseId, DateTime Date, string StockName, double Shares, double Flow, double PriceAvg, string Comment)>
		GetFlowDetailsOrdered(string[] stockNames, DateTime dateFrom, DateTime dateTo)
			=> database.GetFlowDetailsOrdered(stockNames, dateFrom, dateTo);

		public void DeleteFlows(IEnumerable<long> databaseIds) => database.DeleteFlows(databaseIds);

		public int ImportFlows(string csv, string separator) => database.ImportFlows(csv, separator);
	}
}
