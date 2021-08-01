using System.Diagnostics;

namespace JP.InvestCalc
{
	/// <summary>Translates logically between the unsigned UI that distinguishes operations by type,
	/// and the back-end that uses mathematical signs to treat all operations equally.</summary>
	public class Operation
	{
		public readonly string Text;
		public readonly bool
			SharesChange,
			SharesMinus,
			MoneyMinus;
		
		private Operation(string text, bool sharesChange, bool sharesMinus, bool moneyMinus)
		{
			Debug.Assert(!sharesMinus || sharesChange);
			this.Text         = text;
			this.SharesChange = sharesChange;
			this.SharesMinus  = sharesMinus;
			this.MoneyMinus   = moneyMinus;
		}
		
		public readonly static Operation Buy, Sell, Dividend, Cost;
		public readonly static Operation[] All;

		static Operation()
		{
			Buy = new Operation("Buy",
				sharesChange: true , sharesMinus: false, moneyMinus: true );
			Sell = new Operation("Sell",
				sharesChange: true , sharesMinus: true , moneyMinus: false);
			Dividend = new Operation("Dividend",
				sharesChange: false, sharesMinus: false, moneyMinus: false);
			Cost = new Operation("Cost",
				sharesChange: false, sharesMinus: false, moneyMinus: true );

			All = new[] { Buy, Sell, Dividend, Cost };
		}
	}
}
