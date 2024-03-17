using Avalonia.Media.Imaging;
using Avalonia.Tiels.Classes.Platform.Helpers;

namespace Avalonia.Tiels.Classes.Platform.Linux;

public class ThumbnailNsi : ThumbnailCsi
{
	protected override Bitmap GetThumbnailBitmap(string path, params ThumbnailSize[] sizes)
	{
		throw new System.NotImplementedException();
	}

	protected override Bitmap GetDirectoryBitmap(int offest = 0)
	{
		throw new System.NotImplementedException();
	}
}