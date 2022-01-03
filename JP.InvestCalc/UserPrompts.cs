using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	delegate Task AsyncAction<T>(T input);

	static class UserPrompts
	{
		public static Task DisplayError(this Page ui, Exception err)
		{
			return ui.DisplayAlert("ERROR! " + err.GetType().Name, err.Message, "OK");
		}

		public static async Task<bool> PromptConfirmation(this Page ui, string message)
		{
			return await ui.DisplayAlert("Please confirm", message, "Yes", "No");
		}

		public static async Task PromptAndAct(this Page ui, AsyncAction<string> act,
			string title, string message, Keyboard keyboard = null, string initialValue = null)
		{
			string ans = await ui.DisplayPromptAsync(title, message, keyboard:keyboard, initialValue:initialValue);
			if(null == ans) return; // user chose to Cancel
			await act(ans);
		}
	}
}
