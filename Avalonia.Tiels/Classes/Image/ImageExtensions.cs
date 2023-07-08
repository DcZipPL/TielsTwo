namespace Avalonia.Tiels.Classes;

using System.Drawing;
using System.Drawing.Imaging;
using Bitmap = Media.Imaging.Bitmap;

public static class ImageExtensions
{
	public static Bitmap ConvertToAvaloniaBitmap(this Image bitmap)
	{
		System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap);
		var bitmapdata = bitmapTmp.LockBits(new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
		Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
			bitmapdata.Scan0,
			new PixelSize(bitmapdata.Width, bitmapdata.Height),
			new Vector(96, 96),
			bitmapdata.Stride);
		bitmapTmp.UnlockBits(bitmapdata);
		bitmapTmp.Dispose();
		return bitmap1;
	}
}