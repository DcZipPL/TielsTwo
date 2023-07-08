using System;
using Avalonia.Controls;

namespace Avalonia.Tiels.Classes.Style;

public enum TransparencyLevel
{
	None,
	Transparent,
	Blur,
	AcrylicBlur,
	Mica
}

public static class TransparencyLevelConverter
{
	public static WindowTransparencyLevel From(TransparencyLevel level)
	{
		return level switch
		{
			TransparencyLevel.None => WindowTransparencyLevel.None,
			TransparencyLevel.Transparent => WindowTransparencyLevel.Transparent,
			TransparencyLevel.Blur => WindowTransparencyLevel.Blur,
			TransparencyLevel.AcrylicBlur => WindowTransparencyLevel.AcrylicBlur,
			TransparencyLevel.Mica => WindowTransparencyLevel.Mica,
			_ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
		};
	}
}