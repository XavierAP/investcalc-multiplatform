using System.IO;

namespace JP.InvestCalc
{
	public class ModelGateway
	{
		public Calculator Calculator { get; }
		public DataFacade Data { get; }
		public PortfolioData Portfolio { get; }

		internal ViewUpdater Display { get; }

		public string ApiLicenseKey
		{
			get => Portfolio.ApiLicense;
			set => Portfolio.ApiLicense = value;
		}

		public ModelGateway(string dataDirectory, string apiLicenseKey)
		{
			if(string.IsNullOrWhiteSpace(dataDirectory))
				dataDirectory = string.Empty;

			var database = new Database(Path.Combine(dataDirectory, Config.DataFileName));

			Calculator = new Calculator(database);
			Display = new ViewUpdater(Calculator);
			Data = new DataFacade(database);
			Portfolio = new PortfolioData(Display, database);

			ApiLicenseKey = apiLicenseKey;
		}
	}
}
