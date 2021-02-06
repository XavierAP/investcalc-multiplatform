using System;

namespace JP.InvestCalc
{
	public class FlowData
	{
		private readonly Database database;

		internal FlowData(Database database) => this.database = database;

		public FlowEditor GetFlowEditor() => new FlowEditor(database);
	}
}
