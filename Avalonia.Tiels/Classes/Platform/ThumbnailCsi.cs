using System;
using Avalonia.Media.Imaging;

namespace Avalonia.Tiels.Classes.Platform;

public abstract class ThumbnailCsi
{
	public static Bitmap GetThumbnailImage(string path, ThumbnailSize size)
	{
		try
		{
			if (OperatingSystem.IsWindows())
				return new Windows.ThumbnailNsi().GetThumbnailBitmap(path, size);
			if (OperatingSystem.IsLinux())
				return new Linux.ThumbnailNsi().GetThumbnailBitmap(path, size);
		}
		catch (Exception e)
		{
			ErrorHandler.ShowErrorWindow(e, 0x000F);
			throw;
		}

		throw new PlatformNotSupportedException();
	}

	protected abstract Bitmap GetThumbnailBitmap(string path, ThumbnailSize size);
}