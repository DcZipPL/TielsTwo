using System;
using System.IO;
using Avalonia.Media.Imaging;

namespace Avalonia.Tiels.Classes.Platform;

public abstract class ThumbnailCsi
{
	public static Bitmap GetThumbnailImage(string path, ThumbnailSize size)
	{
		try
		{
			FileAttributes attr = File.GetAttributes(path);

			if (OperatingSystem.IsWindows())
				if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
					return new Windows.ThumbnailNsi().GetDirectoryBitmap(path, size);
				else
					return new Windows.ThumbnailNsi().GetThumbnailBitmap(path, size);

			if (OperatingSystem.IsLinux())
				return new Linux.ThumbnailNsi().GetThumbnailBitmap(path, size);
		}
		catch (Exception e)
		{
			throw LoggingHandler.Error(e, nameof(ThumbnailCsi));
		}

		throw LoggingHandler.Error(new PlatformNotSupportedException(), nameof(ThumbnailCsi));
	}

	protected abstract Bitmap GetThumbnailBitmap(string path, ThumbnailSize size);
	protected abstract Bitmap GetDirectoryBitmap(string path, ThumbnailSize size);
}