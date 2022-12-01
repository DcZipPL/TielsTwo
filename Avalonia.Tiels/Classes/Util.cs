using System.Drawing;
using System.Globalization;

namespace Avalonia.Tiels;

public class Util
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
}