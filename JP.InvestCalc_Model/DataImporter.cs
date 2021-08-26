using JP.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JP.InvestCalc
{
	class DataImporter
	{
		private readonly SQLiteConnector connection;

		internal DataImporter(SQLiteConnector connection) => this.connection = connection;

		public int ImportFlows(string csv, CsvProcessor processor)
		{
			int n = 0;
			var records = new Dictionary<string, List<(DateTime Date, double Shares, double Flow, string Comment)>>();

			foreach(var values in processor.Analize(csv))
			{
				if(values.Length < 4) throw new DataImportParseLineException(n, values);

				int i = 0;
				var date = ParseDate(values[i]);
				string stock = values[++i];
				var shares = ParseDouble(values[++i]);
				var flow = ParseDouble(values[++i]);
				var comment = values.Length > ++i ? // comments are optional
					values[i] : null;

				if(!records.TryGetValue(stock, out var stockRecords))
				{
					records.Add(stock, stockRecords =
						new List<(DateTime Date, double Shares, double Flow, string Comment)>(csv.Length / 20 - n));
				}
				stockRecords.Add((date, shares, flow, comment));

				++n;
			}

			foreach(var rec in records.Values)
				rec.Sort(CompareFlows); // ORDER by utcDate, shares DESC
			
			connection.Write(ComposeSql(n, records));
			return n;
		}

		private List<string> ComposeSql(int n,
			Dictionary<string, List<(DateTime Date, double Shares, double Flow, string Comment)>> records)
		{
			var sql = new List<string>(n + records.Count);

			/* We must now check that the records are not absurd
			i.e. that at no time more shares are sold than owned.
			Step 1.- Learn how many were owned before the first operation to be imported:
			*/
			foreach(var stockRecords in records)
			{
				double sharesOwned;
				string stockName = stockRecords.Key;
				using(var query = connection.Select(
$@"SELECT total(Flows.shares) as shares
from Stocks left join Flows
on Flows.stock == Stocks.id
where name == '{stockName}'
and utcDate <= {stockRecords.Value[0].Date.Ticks}
group by Stocks.id"))
				{
					if(query.Read())
						sharesOwned = query.GetDouble();
					else
						sharesOwned = 0;
				}
				sql.Add($"INSERT or IGNORE into Stocks(name) values('{stockName}')");

				// 2.- Check every record:
				Debug.Assert(sharesOwned >= 0);
				foreach(var rec in stockRecords.Value)
				{
					if(-rec.Shares > sharesOwned)
						throw new DataImportValidationException(
$@"Cannot sell more shares than you own! Imported record
of {stockName} on {rec.Date.ToLocalTime().ToShortDateString()}
selling {-rec.Shares} while owning only {sharesOwned} at the time.");

					sharesOwned += rec.Shares;
					Debug.Assert(sharesOwned >= 0);

					// 3.- Add it to the commit:
					sql.Add(
$@"INSERT into Flows values (
{rec.Date.Ticks},
(select id from Stocks where name = '{stockName}'),
{rec.Shares}, {rec.Flow},
{(string.IsNullOrWhiteSpace(rec.Comment) ? "NULL" : $"'{rec.Comment}'")}
)");
				}
			}
			return sql;
		}

		private static int CompareFlows(
			(DateTime Date, double Shares, double Flow, string Comment) a,
			(DateTime Date, double Shares, double Flow, string Comment) b)
		{
			if(a.Date < b.Date)
				return -1;
			else if(a.Date > b.Date)
				return 1;
			else if(a.Shares > b.Shares)
				return -1;
			else if(a.Shares < b.Shares)
				return 1;
			else
				return 0;
		}

		private static double ParseDouble(string text)
		{
			if(!double.TryParse(text, out double number))
				throw new DataImportParseValueException(text, "number");

			return number;
		}

		private static DateTime ParseDate(string text)
		{
			if(!DateTime.TryParse(text, out DateTime date))
				throw new DataImportParseValueException(text, "date");

			return date.ToUniversalTime();
		}
	}
}
