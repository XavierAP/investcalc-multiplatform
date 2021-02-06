using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JP.InvestCalc
{
	public sealed partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			MainPage = new PageMain();
		}

		//protected override void OnStart()
		//{
		//	// Handle when your app starts
		//}

		//protected override void OnSleep()
		//{
		//	// Handle when your app sleeps
		//}

		//protected override void OnResume()
		//{
		//	// Handle when your app resumes
		//}
	}
}
