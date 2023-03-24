using System;
using Avalonia.Media.Imaging;

namespace Avalonia.Tiels.Classes.Platform;

public abstract class ThumbnailCsi
{
	public static Bitmap GetThumbnailImage(string path, int width, int height, ThumbnailSize size)
	{
		if (OperatingSystem.IsWindows())
			return new Windows.ThumbnailNsi().GetThumbnailBitmap(path, width, height);
		if (OperatingSystem.IsLinux())
			return new Linux.ThumbnailNsi().GetThumbnailBitmap(path, width, height);
		throw new NotImplementedException();
	}

	protected abstract Bitmap GetThumbnailBitmap(string path, int width, int height);
}