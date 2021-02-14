using System;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	static class WinFormsExtensions
	{
		public static void Display(this Exception err) => MessageBox.Show(err.Message,
			err.GetType().FullName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

		public static void TryIfOk(this FileDialog dlg, Action<string> actOnFileName)
		{
			var ans = dlg.ShowDialog();
			if(ans != DialogResult.OK) return;
			try
			{
				actOnFileName(dlg.FileName);
			}
			catch(Exception err)
			{
				err.Display();
			}
		}
	}
}
