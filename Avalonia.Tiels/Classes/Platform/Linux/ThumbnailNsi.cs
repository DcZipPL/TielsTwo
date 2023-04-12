using Avalonia.Media.Imaging;

namespace Avalonia.Tiels.Classes.Platform.Linux;

public class ThumbnailNsi : ThumbnailCsi
{
	protected override Bitmap GetThumbnailBitmap(string path, ThumbnailSize size)
	{
		throw new System.NotImplementedException();
	}

	protected override Bitmap GetDirectoryBitmap(string path, ThumbnailSize size)
	{
		throw new System.NotImplementedException();
	}
}