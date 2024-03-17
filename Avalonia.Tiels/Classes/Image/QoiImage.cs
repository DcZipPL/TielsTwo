using System;
using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DotNext;

namespace Avalonia.Tiels.Classes.Image;

public class QoiImage
{
	public static Result<Bitmap> DecodeFromBytes(byte[] bytes)
	{
		var decoder = new QOIDecoder();
		var success = decoder.Decode(encoded: bytes, encodedSize: bytes.Length);
		
		if (!success)
			return new Result<Bitmap>(new Exception("Couldn't decode QOI image."));
		
		int width = decoder.GetWidth();
		int height = decoder.GetHeight();
		WriteableBitmap bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888);
						
		using (var bitmapLock = bitmap.Lock())
		{
			IntPtr buffer = bitmapLock.Address;
							
			int[] pixels = decoder.GetPixels();
			int pixelCount = width * height;
			for (int i = 0; i < pixelCount; i++)
			{
				// Extract ARGB components from the integer value
				int pixel = pixels[i];
				byte alpha = (byte)((pixel >> 24) & 0xFF);
				byte red = (byte)((pixel >> 16) & 0xFF);
				byte green = (byte)((pixel >> 8) & 0xFF);
				byte blue = (byte)(pixel & 0xFF);

				// Write the pixel values to the bitmap buffer
				Marshal.WriteByte(buffer, i * 4 + 0, blue);
				Marshal.WriteByte(buffer, i * 4 + 1, green);
				Marshal.WriteByte(buffer, i * 4 + 2, red);
				Marshal.WriteByte(buffer, i * 4 + 3, alpha);
			}
		}
			
		return new Result<Bitmap>(bitmap);
	}
}