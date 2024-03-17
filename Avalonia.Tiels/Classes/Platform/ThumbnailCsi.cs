using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Tiels.Classes.Platform.Helpers;
using DotNext;

namespace Avalonia.Tiels.Classes.Platform;

public abstract class ThumbnailCsi
{
	public static Result<Bitmap> GetThumbnailImage(string path, params ThumbnailSize[] sizes)
	{
		try
		{
			FileAttributes attr = File.GetAttributes(path);

			if (OperatingSystem.IsWindows())
				if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
					return new Windows.ThumbnailNsi().GetDirectoryBitmap();
				else
					return new Windows.ThumbnailNsi().GetThumbnailBitmap(path, sizes);

			if (OperatingSystem.IsLinux())
				return new Linux.ThumbnailNsi().GetThumbnailBitmap(path, sizes);
		}
		catch (Exception e)
		{
			throw LoggingHandler.Error(e, nameof(ThumbnailCsi));
		}

		throw LoggingHandler.Error(new PlatformNotSupportedException(), nameof(ThumbnailCsi));
	}

	public static Bitmap GetShellIcon(int offest = 3)
	{
		if (OperatingSystem.IsWindows())
			return new Windows.ThumbnailNsi().GetDirectoryBitmap(offest);
		else if (OperatingSystem.IsLinux())
			return new Linux.ThumbnailNsi().GetDirectoryBitmap(offest);

		throw LoggingHandler.Error(new PlatformNotSupportedException(), nameof(GetShellIcon));
	}

	protected abstract Result<Bitmap> GetThumbnailBitmap(string path, params ThumbnailSize[] sizes);
	protected abstract Bitmap GetDirectoryBitmap(int offest = 3);
}