using System.IO;
using SkiaSharp;

namespace Avalonia.Tiels.Classes.Image;

public static class ImageExtensions
{
	public static Media.Imaging.Bitmap ConvertToAvaloniaBitmap(this SKBitmap skiaBitmap)
	{
		using var stream = new MemoryStream();
		using var skiaImage = SKImage.FromBitmap(skiaBitmap);
		skiaImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
		stream.Position = 0;
		var bitmap = new Media.Imaging.Bitmap(stream);
		return bitmap;
	}
}