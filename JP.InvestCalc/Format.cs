﻿using Xamarin.Forms;

namespace JP.InvestCalc
{
	static class Format
	{
		public static readonly Color BackgroundColor = Color.AliceBlue;
		public static readonly Color FillColor       = Color.LightBlue;
		public static readonly Color LineColor       = Color.Blue;
		
		public static string FormatShares(this double value) => value == 0 ? null : value.ToString("#,0.##");
		public static string FormatMoneyPositive(this double value) => value == 0 ? null : value.ToString("#,0.00");
		public static string FormatMoneyPlusMinus(this double value) => value.ToString("+#,0.00;-#,0.00;0");
		public static string FormatPerCent(this double per1) => per1.ToString("+#,0.##%;-#,0.##%;0%");

		public static Label SetFontSizeMediumSmall(this Label lbl)
		{
			lbl.FontSize = FontSizeMediumSmall;
			return lbl;
		}
		private readonly static double FontSizeMediumSmall = 0.5 * (
			Device.GetNamedSize(NamedSize.Small, typeof(Label)) +
			Device.GetNamedSize(NamedSize.Medium, typeof(Label)) );
	}
}
