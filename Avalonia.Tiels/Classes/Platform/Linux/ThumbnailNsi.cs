using Avalonia.Media.Imaging;
using Avalonia.Tiels.Classes.Platform.Helpers;
using DotNext;

namespace Avalonia.Tiels.Classes.Platform.Linux;

public class ThumbnailNsi : ThumbnailCsi
{
	protected override Result<Bitmap> GetThumbnailBitmap(string path, params ThumbnailSize[] sizes)
	{
		throw new System.NotImplementedException();
	}

	protected override Bitmap GetDirectoryBitmap(int offest = 0)
	{
		throw new System.NotImplementedException();
	}
}