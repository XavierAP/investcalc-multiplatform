using System;
using System.IO;

namespace JP.InvestCalc
{
	public class ModelGateway
	{
		public Calculator Calculator { get; }
		public FlowData Flows { get; }
		public PortfolioData Portfolio { get; }

		public string DataFilePath { get; }

		internal ViewUpdater Display { get; }

		public ModelGateway(string dataDirectory, string priceAPILicense)
		{
			if(string.IsNullOrWhiteSpace(dataDirectory))
				dataDirectory = string.Empty;

			DataFilePath = Path.Combine(dataDirectory, Config.DataFileName);
			var database = new Database(DataFilePath);

			Calculator = new Calculator(database);
			Display = new ViewUpdater(Calculator);
			Flows = new FlowData(database);
			Portfolio = new PortfolioData(Display, database, priceAPILicense);
		}
	}
}
