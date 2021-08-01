using System;
using Xamarin.Forms;

namespace JP.InvestCalc
{
	static class Format
	{
		public static readonly Color BackgroundColor = Color.AliceBlue;
		public static readonly Color FillColor       = Color.LightBlue;
		public static readonly Color LineColor       = Color.Blue;

		const string Pad = " ";

		public static string FormatMoney(this double value) => value.ToString("N2") + Pad;
		
		public static string FormatShares(this double shares) => $"({Math.Round(shares, 4).ToString()}){Pad}";
		
		public static string FormatPerCent(this double per1) => per1.ToString("P" + Config.PrecisionPerCent.ToString());
		
		public static string? ValueOnUnknownPrice(double shares) => shares == 0 ? 0d.FormatMoney() : null;
	}
}
