using System.Threading.Tasks;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	static class UserPrompts
	{
		public static async Task<bool> PromptConfirmation(this Page ui, string message)
		{
			return await ui.DisplayAlert("Please confirm", message, "Yes", "No");
		}
	}
}
