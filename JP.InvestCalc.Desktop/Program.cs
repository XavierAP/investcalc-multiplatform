using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace JP.InvestCalc
{
	internal static class Program
	{
		[STAThread]
		static void Main()
		{
			var model = new ModelGateway(GetDataFolder(), Properties.Settings.Default.apiLicense);

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormMain(model));
		}

		private static string GetDataFolder()
		{
			string folder = Properties.Settings.Default.dataFolder;

			if(string.IsNullOrWhiteSpace(folder))
				return GetDataFolderDefault();
			
			try { Directory.CreateDirectory(folder); }
			catch(Exception err)
			{
				MessageBox.Show($"Error creating custom data folder:\n{folder}\nDefaulting to %AppData%.\n\n{err.GetType().Name}\n{err.Message}",
					Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return GetDataFolderDefault();
			}
			Debug.Assert(Directory.Exists(folder));
			return folder;
		}

		private static string GetDataFolderDefault() => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"JP", Config.AppName);
	}
}
