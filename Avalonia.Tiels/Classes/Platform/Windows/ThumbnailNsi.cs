using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Media.Imaging;
using Avalonia.Tiels.Classes.Image;
using Avalonia.Tiels.Classes.Platform.Helpers;
using SkiaSharp;
using static Avalonia.Tiels.Classes.Platform.Windows.ThumbnailShellNsi;

namespace Avalonia.Tiels.Classes.Platform.Windows;

[SupportedOSPlatform("windows")]
public class ThumbnailNsi : ThumbnailCsi
{
	public static Bitmap ConvertToBitmap(System.Drawing.Image bitmap)
	{
		System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap);
		var bitmapdata = bitmapTmp.LockBits(new System.Drawing.Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
			bitmapdata.Scan0,
			new PixelSize(bitmapdata.Width, bitmapdata.Height),
			new Vector(96, 96),
			bitmapdata.Stride);
		bitmapTmp.UnlockBits(bitmapdata);
		bitmapTmp.Dispose();
		return bitmap1;
	}
	
	protected override Bitmap GetThumbnailBitmap(string path, ThumbnailSize size)
	{
		var bitmap = new SKBitmap(new SKImageInfo(1, 1)).ConvertToAvaloniaBitmap();
		try
		{
			(int, IntPtr) hIcon = GetIconPointer(GetIconIndex(path), size);
			if (hIcon.Item1 != 0)
				throw new NullReferenceException("Failed to extract icon");
			
			using System.Drawing.Icon ico = System.Drawing.Icon.FromHandle(hIcon.Item2);
			bitmap = ConvertToBitmap(ico.ToBitmap());
			var hResult = Shell32.DestroyIcon(hIcon.Item2);
			if (hResult == 0)
				throw new NullReferenceException("Failed to destroy icon");
		}
		catch (Exception e)
		{
			LoggingHandler.Warn(nameof(GetThumbnailBitmap), "Failed to load thumbnail from file: " + path + "\n" + e);
		}

		return bitmap;
	}


	protected override Bitmap GetDirectoryBitmap(int offest = 3)
	{
		string fileName = @$"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\imageres.dll"; // path to imageres.dll
		IntPtr hIcon = ExtractIcon(IntPtr.Zero, fileName, offest); // get icon from imageres.dll

		if (hIcon != IntPtr.Zero)
		{
			using System.Drawing.Icon ico = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
			var bitmap = ConvertToBitmap(ico.ToBitmap());
			var hResult = Shell32.DestroyIcon(hIcon);
			if (hResult == 0)
				throw LoggingHandler.Error(new Exception("Failed to extract icon"), nameof(GetDirectoryBitmap) + "@0");

			return bitmap;
		}
		else
		{
			throw LoggingHandler.Error(new Exception("Failed to extract icon"), nameof(GetDirectoryBitmap) + "@1");
			// error handling
		}
	}

	private static int GetIconIndex(string pszFile)
	{
		SHFILEINFO sfi = new SHFILEINFO();
		Shell32.SHGetFileInfo(pszFile,
			0,
			ref sfi,
			(uint)Marshal.SizeOf(sfi),
			(uint)(SHGFI.SysIconIndex | SHGFI.LargeIcon | SHGFI.UseFileAttributes));
		return sfi.iIcon;
	}

	private static (int, IntPtr) GetIconPointer(int imagePtr, ThumbnailSize size)
	{
		IImageList? spiml = null;
		Guid guil = new Guid(IID_IImageList);

		int hResult = Shell32.SHGetImageList((int)size, ref guil, ref spiml);
		
		if (hResult != 0)
			return (hResult, IntPtr.Zero);
		
		IntPtr hIcon = IntPtr.Zero;
		spiml.GetIcon(imagePtr, Shell32.ILD_TRANSPARENT | Shell32.ILD_IMAGE, ref hIcon);

		return (0, hIcon);
	}

	[DllImport("Shell32.dll", EntryPoint = "ExtractIcon")]
    static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
}