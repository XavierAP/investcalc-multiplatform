namespace JP.InvestCalc
{
	public class DataFacade
	{
		private readonly Database Database;

		internal DataFacade(Database database) => this.Database = database;

		public FlowEditor GetFlowEditor() => new FlowEditor(Database);

		public string FilePath => Database.FilePath;
		public string QueryStocks => Database.QueryStocks;
	}
}
