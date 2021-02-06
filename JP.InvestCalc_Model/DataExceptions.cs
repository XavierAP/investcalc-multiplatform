using System;

namespace JP.InvestCalc
{
	public class DataException : Exception
	{
		protected DataException(string message)
			: base(message) { }
	}

	public sealed class DataImportParseLineException : DataException
	{
		public DataImportParseLineException(string line)
			: base("Cannot parse columns from line: " + line) { }
	}

	public sealed class DataImportParseValueException : DataException
	{
		public DataImportParseValueException(string text, string parseType)
			: base($"Cannot parse as {parseType}:\n{text}") { }
	}

	public sealed class DataImportValidationException : DataException
	{
		public DataImportValidationException(string message)
			: base(message) { }
	}
}
