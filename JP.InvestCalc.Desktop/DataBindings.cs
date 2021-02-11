using JP.SQLite;
using System;
using System.Data;

namespace JP.InvestCalc
{
	class DataBindings : IDisposable
	{
		public DataTable StockBinding => BinderStocks.DataTable;

		public void Update(DataTable binding)
		{
			if(ReferenceEquals(binding, BinderStocks.DataTable))
				BinderStocks.Update();
			else
				throw new InvalidOperationException($"{nameof(DataBindings)}.{nameof(Update)} must be called with an object previously returned by this same {nameof(DataBindings)} object.");
		}

		public void Dispose() => BinderStocks.Dispose();

		internal DataBindings(ModelGateway model)
		{
			BinderStocks = new SQLiteBinder(model.Data.FilePath, model.Data.QueryStocks);
		}

		readonly SQLiteBinder BinderStocks;
	}
}
