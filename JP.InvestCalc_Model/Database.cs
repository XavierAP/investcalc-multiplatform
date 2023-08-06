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

		internal Database(string dataFile)
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


		public int ImportFlows(string csv, CsvProcessor processor)
		{
			var n = new DataImporter(Connection).ImportFlows(csv, processor);
			IsPortfolioChanged = true;
			return n;
		}

		public List<(string StockName, double TotalShares)>
		GetPortfolio()
		{
			var ans = new List<(string StockName, double TotalShares)>();
			using(var query = Connection.Select(
@"SELECT Stocks.name, total(Flows.shares) as shares
from Stocks left join Flows
on Flows.stock = Stocks.id
group by Stocks.id
order by name"))
			{
				while(query.Read())
					ans.Add((query.GetString(), query.GetDouble()));
			}
			IsPortfolioChanged = false;
			return ans;
		}

		
		public void SetStockName(string oldName, string newName)
		{
			Connection.Write($"UPDATE Stocks SET name = '{newName}' WHERE name = '{oldName}'");
		}

		public void SetFetchCode(string stockName, string code)
		{
			Connection.Write($"UPDATE Stocks SET fetchCodes = '{code}' WHERE name = '{stockName}'");
		}

		/// <summary>Returns defined (excluding NULL) fetch codes.</summary>
		/// <param name="stockNames">Limited to these stocks.
		/// If null, for all stocks.</param>
		public List<(string Name, string Code)>
		GetFetchCodes(IEnumerable<string> stockNames)
		{
			var ans = new List<(string Name, string Code)>();
			using(var query = Connection.Select(
$@"SELECT name, fetchCodes
from Stocks
where {(stockNames==null ? null :
	$"name in ('{string.Join("', '", stockNames)}') and ")}
fetchCodes is not NULL"))
			{
				while(query.Read())
					ans.Add((query.GetString(), query.GetString()));
			}
			return ans;
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

			if(shares != 0 || money != 0) sql.Add(
$@"INSERT into Flows values (
{day.ToUniversalTime().Ticks},
(select id from Stocks where name = '{stockName}'),
{shares}, {money},
{(string.IsNullOrWhiteSpace(comment) ? "NULL" : $"'{comment}'")}
)");
			Connection.Write(sql);
		}


		public List<(double Cash, DateTime Day)>
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

			var ans = new List<(double Cash, DateTime Day)>();
			using(var query = Connection.Select(sql.ToString()))
				while(query.Read())
					ans.Add((
						query.GetDouble() ,
						new DateTime(query.GetInt64(), DateTimeKind.Utc) ));

			return ans;
		}


		public List<(long Id, DateTime Date, string StockName, double Shares, double Flow, double PriceAvg, string Comment)>
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

			var ans = new List<(long Id, DateTime Date, string StockName, double Shares, double Flow, double PriceAvg, string Comment)>();
			using(var query = Connection.Select(sql.ToString()))
				while(query.Read())
				{
					var dbId = query.GetInt64();
					var date = new DateTime(query.GetInt64(), DateTimeKind.Utc).ToLocalTime();
					var stockName = query.GetString();
					var shares = query.GetDouble();
					var flow = query.GetDouble();
					var priceAvg = Math.Round(-flow / shares, 2);
					string comment = query.IsNullNext() ? null : query.GetString();
					
					ans.Add((dbId, date, stockName, shares, flow, priceAvg, comment));
				}
			return ans;
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
	name text unique collate nocase not null, -- Human-friendly name; unique index.
	fetchCodes text
);
create table Flows ( -- Record of purchases, sales, dividends, holding costst, etc.
	utcDate integer not null,
	stock integer not null references Stocks(id)
		on delete cascade on update cascade,
	shares real not null, -- Amount of shares acquired (+) or sold or forfeit (-). May be non integer in case of e.g. physical metals.
	flow real not null, -- Money received (+) or paid (-); in principle opposite sign to 'shares'.
	comment text
);
create index idxFlow2Stock
on Flows(stock);
create index idxFlowDate
on Flows(utcDate);
";
	}
}
