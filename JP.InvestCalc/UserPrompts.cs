using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	static class UserPrompts
	{
		public static Task DisplayError(this Page ui, string err)
		{
			return ui.DisplayAlert("ERROR! ", err, "OK");
		}
		public static Task DisplayError(this Page ui, Exception err)
		{
			return ui.DisplayAlert("ERROR! " + err.GetType().Name, err.Message, "OK");
		}

		public static async Task<bool> PromptConfirmation(this Page ui, string message)
		{
			return await ui.DisplayAlert("Please confirm", message, "Yes", "No");
		}
	}
}
