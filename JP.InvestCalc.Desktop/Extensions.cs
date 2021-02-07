using System;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	static class Extensions
	{
		public static void Display(this Exception err) => MessageBox.Show(err.Message,
			Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	}
}
