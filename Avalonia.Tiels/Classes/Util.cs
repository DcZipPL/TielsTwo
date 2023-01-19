using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Avalonia.Svg;

namespace Avalonia.Tiels;

public static class Util
{
	public static Color ColorFromHex(string? colorcode)
	{
		if (colorcode == null) return Color.FromArgb(0x55000000);
		
		colorcode = colorcode.TrimStart('#');
		
		if (colorcode.Length == 6)
			return Color.FromArgb(85, // hardcoded opaque
				int.Parse(colorcode.Substring(0,2), NumberStyles.HexNumber),
				int.Parse(colorcode.Substring(2,2), NumberStyles.HexNumber),
				int.Parse(colorcode.Substring(4,2), NumberStyles.HexNumber));
		else // assuming length of 8
			return Color.FromArgb(
				int.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber),
				int.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber),
				int.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber),
				int.Parse(colorcode.Substring(6, 2), NumberStyles.HexNumber));
	}
	
	public static string ColorToHex(Color c)
	{
		return "#" + c.A.ToString("X2") + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
	}

	public static CultureInfo[] ImplementedCultures()
	{
		// TODO: Check of it is possible to prevent hardcoding
		return new[] {
			CultureInfo.GetCultureInfo("en-US"),
			CultureInfo.GetCultureInfo("en-UK"),
			CultureInfo.GetCultureInfo("pl-PL")
		};
	}

	public static Avalonia.Controls.Image SetSvgImage(string iconPath, Avalonia.Controls.Image image)
	{
		var svg = new SvgImage
		{
			Source = SvgSource.Load(iconPath?? "/Assets/Icons/out/alert-octagon.svg",
				new Uri("avares://Avalonia.Tiels"+iconPath))
		};
		image.Source = svg;
		return image;
	}
}