﻿using System;
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
		
		public static string FormatShares(this double shares) => Math.Round(shares, 4).ToString("N") + Pad;
		public static string ReverseFormatShares(this string shares) => shares[1..^(1 + Pad.Length)];
		
		public static string FormatPerCent(this double per1) => per1.ToString("P2");
		
		public static string? ValueOnUnknownPrice(double shares) => shares == 0 ? 0d.FormatMoney() : null;
	}
}
