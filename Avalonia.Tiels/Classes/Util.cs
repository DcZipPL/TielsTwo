using System.Globalization;
using Avalonia.Media;

namespace Avalonia.Tiels.Classes;

public static class Util
{
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
}