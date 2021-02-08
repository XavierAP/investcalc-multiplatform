using System;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	static class WinFormsExtensions
	{
		public static void Display(this Exception err) => MessageBox.Show(err.Message,
			err.GetType().FullName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	}
}
