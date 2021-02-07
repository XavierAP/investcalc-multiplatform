using JP.SQLite;
using System;
using System.Data;

namespace JP.InvestCalc
{
	class DataBindings
	{
		public DataTable GetStockBinding() => BinderStocks.DataTable;

		public void Update(DataTable binding)
		{
			if(ReferenceEquals(binding, BinderStocks.DataTable))
				BinderStocks.Update();
			else
				throw new InvalidOperationException($"{nameof(DataBindings)}.{nameof(Update)} must be called with an object previously returned by this same {nameof(DataBindings)} object.");
		}

		public DataBindings(ModelGateway model, byte numOfReadOnlyCols = 1)
		{
			BinderStocks = new SQLiteBinder(
				model.Data.FilePath,
				model.Data.QueryStocks,
				numOfReadOnlyCols);
		}

		readonly SQLiteBinder BinderStocks;
	}
}
