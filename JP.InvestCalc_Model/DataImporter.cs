using JP.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace JP.InvestCalc
{
	class DataImporter
	{
		private readonly SQLiteConnector connection;

		internal DataImporter(SQLiteConnector connection) => this.connection = connection;

		public int ImportFlows(string csv, string separator)
		{
			if(string.IsNullOrEmpty(separator)) throw new ArgumentNullException(nameof(separator));
			if(string.IsNullOrWhiteSpace(csv)) throw new ArgumentNullException(nameof(csv));

			int n = 0;
			var records = new Dictionary<string, List<(DateTime Date, double Shares, double Flow, string Comment)>>();

			var seps = new[] { separator };

			string line;
			using(var reader = new StringReader(csv))
				while(null != (line = reader.ReadLine()))
				{
					if(string.IsNullOrWhiteSpace(line)) continue;

					var values = line.Split(seps,
						StringSplitOptions.RemoveEmptyEntries); // allow the user to have entered e.g. several tabs in a row for visual reasons; only the last column (comment) may optionally be empty

					if(values.Length < 4)
						throw new DataImportParseLineException(line);

					int i = 0;

					var dateText = values[i].Trim();
					if(!DateTime.TryParse(dateText, out DateTime date))
						throw new DataImportParseValueException(dateText, "date");

					Debug.Assert(date.Kind == DateTimeKind.Unspecified); // from TryParse(); considered local
					date = date.Date.ToUniversalTime();

					string stock = values[++i].Trim();

					var sharesText = values[++i].Trim();
					if(!double.TryParse(sharesText, out double shares))
						throw new DataImportParseValueException(sharesText, "number");

					var flowText = values[++i].Trim();
					if(!double.TryParse(flowText, out double flow))
						throw new DataImportParseValueException(flowText, "number");

					var comment = values.Length > ++i ? // comments are optional
						values[i].Trim() : null;

					if(!records.TryGetValue(stock, out var stockRecords))
						records.Add(stock,
						stockRecords = new List<(DateTime Date, double Shares, double Flow, string Comment)>(csv.Length / 20 - n)
						);
					stockRecords.Add((date, shares, flow, comment));

					++n;
				}

			foreach(var rec in records.Values)
				rec.Sort(CompareFlows); // ORDER by utcDate, shares DESC

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
					if(query.Rows.Count > 0)
					{
						Debug.Assert(1 == query.Rows.Count);
						sharesOwned = (double)query.Rows[0][0];
					}
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

			// All Korrect, finally! write to the database:
			connection.Write(sql);
			return n;
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
	}
}
