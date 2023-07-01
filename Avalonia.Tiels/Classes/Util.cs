using System;
using System.Globalization;
using Avalonia.Media;
using Avalonia.Svg;

namespace Avalonia.Tiels.Classes;

public static class Util
{
	public static readonly Color TILE_DARK_COLOR = Color.Parse("#50000000");
	public static readonly Color TILE_LIGHT_COLOR = Color.Parse("#50ffffff");

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