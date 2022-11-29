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
			return Color.FromArgb(255, // hardcoded opaque
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
}