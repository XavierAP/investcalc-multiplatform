using System;

namespace JP.InvestCalc
{
	public static class Config
	{
		public const string AppName = "InvestCalc";
		public const string DataFileName = AppName + ".data";
		
		public static string GetDataFolder() => Environment.GetFolderPath(
			Environment.SpecialFolder.MyDocuments );

		public const string DefaultCsvSeparator = "\t";
	}
}
