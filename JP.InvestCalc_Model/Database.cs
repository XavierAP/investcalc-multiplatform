using JP.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace JP.InvestCalc
{
	class Database
	{
		public string FilePath { get; }
		private readonly SQLiteConnector Connection;

		public bool IsPortfolioChanged { get; private set; } = true;

		public const string QueryStocks = "SELECT * FROM Stocks";

		public Database(string dataFile)
		{
			FilePath = dataFile;
			Connection = File.Exists(dataFile) ?
				new SQLiteConnector(dataFile, true) :
				CreateEmptyDatabase(dataFile) ;
		}
		private SQLiteConnector CreateEmptyDatabase(string dataFile)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(dataFile));
			var connection = new SQLiteConnector(dataFile, false);
			connection.Write(ddl);
			return connection;
		}


		public int ImportFlows(string csv, string separator)
		{
			var n = new DataImporter(Connection).ImportFlows(csv, separator);
			IsPortfolioChanged = true;
			return n;
		}

		public IEnumerable<(string StockName, double TotalShares)>
		GetPortfolio()
		{
			using(var table = Connection.Select(
@"SELECT Stocks.name, total(Flows.shares) as shares
from Stocks left join Flows
on Flows.stock = Stocks.id
group by Stocks.id
order by name"))
			{
				foreach(DataRow record in table.Rows)
					yield return ( (string)record[0], (double)record[1] );
			}
			IsPortfolioChanged = false;
		}


		/// <summary>Returns defined (excluding NULL) fetch codes.</summary>
		/// <param name="stockNames">Limited to these stocks.
		/// If null, for all stocks.</param>
		public IEnumerable<(string Name, string Code)>
		GetFetchCodes(IEnumerable<string> stockNames)
		{
			using(var table = Connection.Select(
$@"SELECT name, fetchCodes
from Stocks
where {(stockNames==null ? null :
	$"name in ('{string.Join("', '", stockNames)}') and ")}
fetchCodes is not NULL"))
			{
				foreach(DataRow record in table.Rows)
					yield return ( (string)record[0], (string)record[1] );
			}
		}


		/// <summary>Records a new operation into the database.</summary>
		/// <param name="newStock">Whether this stock was traded before.</param>
		/// <param name="stockName">What.</param>
		/// <param name="day">When.</param>
		/// <param name="shares">How many.</param>
		/// <param name="money">How much.</param>
		/// <param name="comment">Description of the operation for the user's own recollection.</param>
		public void OpRecord(bool newStock, string stockName, DateTime day,
			double shares, double money, string comment)
		{
			var sql = new List<string>(2);

			if(newStock) sql.Add($"INSERT into Stocks(name) values('{stockName}')");

			sql.Add(
$@"INSERT into Flows values (
{day.ToUniversalTime().Ticks},
(select id from Stocks where name = '{stockName}'),
{shares}, {money},
{(string.IsNullOrWhiteSpace(comment) ? "NULL" : $"'{comment}'")}
)");
			Connection.Write(sql);
		}


		public IEnumerable<(double Cash, DateTime Day)>
		GetFlows(params string[] stockNames)
		{
			var sql = new StringBuilder(
@"SELECT flow, utcDate
from Flows, Stocks ON Flows.stock = Stocks.id
");
			if(stockNames != null && stockNames.Length > 0)
				sql.Append("where ").Append(string.Join(" OR ",
					from name in stockNames
					select $"name = '{name}'"));

			using(var table = Connection.Select(sql.ToString()))
				return from DataRow record in table.Rows select (
					(double)record[0],
					new DateTime((long)record[1], DateTimeKind.Utc) );
		}


		public IEnumerable<(long Id, DateTime Date, string StockName, double Shares, double Flow, double PriceAvg, string Comment)>
		GetFlowDetailsOrdered(string[] stockNames, DateTime dateFrom, DateTime dateTo)
		{
			dateFrom = dateFrom.Date;
			dateTo   = dateTo  .Date;
			Debug.Assert(dateFrom <= dateTo);

			var sql = new StringBuilder(
@"SELECT Flows.rowid, utcDate, name, shares, flow, comment
from Flows, Stocks ON Flows.stock = Stocks.id
");
			sql.AppendLine(string.Format("where utcDate >= {0} AND utcDate <= {1}",
				dateFrom.ToUniversalTime().Ticks.ToString() ,
				dateTo  .ToUniversalTime().Ticks.ToString() ));

			if(stockNames != null && stockNames.Any())
				sql.Append("AND ( ").Append(string.Join(" OR ",
					from name in stockNames
					select $"name = '{name}'")).AppendLine(" )");

			sql.Append("order by utcDate, shares DESC"); // TL;DR why order by shares DESC: corner case of several operations, with the same stock, in the same day. Chronological order may be lost because dates are rounded down to days; and it would create an absurd history, if the user deleted manually (see DeleteFlows, FormHistory.DoDelete and FormHistory.Table_CellMouseDown) a flow buying shares, so that later flows selling put the total owned into negative.

			using(var table = Connection.Select(sql.ToString()))
			{
				foreach(DataRow record in table.Rows)
				{
					var dbId = (long)record[0];
					var date = new DateTime((long)record[1], DateTimeKind.Utc).ToLocalTime();
					var stockName = (string)record[2];
					var shares = (double)record[3];
					var flow = (double)record[4];
					var priceAvg = Math.Round(-flow / shares, 2);
					string comment = record[5] == DBNull.Value ? null : (string)record[5];
					
					yield return (dbId, date, stockName, shares, flow, priceAvg, comment);
				}
			}
		}


		public void DeleteFlows(IEnumerable<long> ids)
		{
			if(ids == null || !ids.Any()) return;
			var sql = $"DELETE from Flows where rowid IN ( {string.Join(", ", ids)} )";
			Connection.Write(sql);
			IsPortfolioChanged = true;
		}

		private const string ddl =
@"pragma foreign_keys = ON;

create table Stocks ( -- Portfolio of stocks, shares, bonds, metals, etc.
	id integer primary key,
	name text unique collate nocase not null -- Human-friendly name; unique index.
);
create table Flows ( -- Record of purchases, sales, dividends, holding costst, etc.
	utcDate integer not null,
	stock integer not null references Stocks(id)
		on delete restrict on update cascade,
	shares real not null, -- Amount of shares acquired (+) or sold or forfeit (-). May be non integer in case of e.g. physical metals.
	flow real not null, -- Money received (+) or paid (-); in principle opposite sign to 'shares'.
	comment text
);
create index idxFlow2Stock
on Flows(stock);
create index idxFlowDate
on Flows(utcDate);

alter table Stocks add
	fetchCodes text;
";
	}
}
