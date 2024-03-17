using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Media.Imaging;
using Avalonia.Tiels.Classes.Image;
using Avalonia.Tiels.Classes.Platform.Helpers;
using DotNext;
using SkiaSharp;
using static Avalonia.Tiels.Classes.Platform.Windows.ThumbnailShellNsi;

namespace Avalonia.Tiels.Classes.Platform.Windows;

[SupportedOSPlatform("windows")]
public class ThumbnailNsi : ThumbnailCsi
{
	public static Bitmap ConvertToBitmap(System.Drawing.Image bitmap)
	{
		System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap);
		var bitmapData = bitmapTmp.LockBits(new System.Drawing.Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		Bitmap converted = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
			bitmapData.Scan0,
			new PixelSize(bitmapData.Width, bitmapData.Height),
			new Vector(96, 96),
			bitmapData.Stride);

		bitmapTmp.UnlockBits(bitmapData);
		bitmapTmp.Dispose();
		return converted;
	}
	
	protected override Bitmap GetThumbnailBitmap(string path, params ThumbnailSize[] sizes)
	{
		var bitmap = new SKBitmap(new SKImageInfo(1, 1)).ConvertToAvaloniaBitmap();
		try
		{
			Result<IntPtr, E_SHGIL> hIcon = GetIconPointer(GetIconIndex(path), sizes);
			IntPtr header = hIcon switch
			{
				{ IsSuccessful: true } => hIcon.Value,
				{ IsSuccessful: false, Error: E_SHGIL.E_OUTOFMEMORY } => throw new OutOfMemoryException("GetIconPointer returned out of memory, " + hIcon.Error),
				{ IsSuccessful: false, Error: _ } => throw new Exception("GetIconPointer returned out of memory, " + hIcon.Error)
			};
			
			using System.Drawing.Icon ico = System.Drawing.Icon.FromHandle(header);
			bitmap = ConvertToBitmap(ico.ToBitmap());
			var hResult = Shell32.DestroyIcon(hIcon.Value);
			if (hResult == 0)
				throw new Exception("Failed to destroy icon");
		}
		catch (Exception e)
		{
			LoggingHandler.Warn(nameof(GetThumbnailBitmap), "Failed to load thumbnail from file: " + path + "\n" + e);
		}

		return bitmap;
	}

	// TODO: Use rust like error handling
	protected override Bitmap GetDirectoryBitmap(int offest = 3)
	{
		string fileName = @$"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\imageres.dll"; // path to imageres.dll
		IntPtr hIcon = ExtractIcon(IntPtr.Zero, fileName, offest); // get icon from imageres.dll

		if (hIcon != IntPtr.Zero)
		{
			using System.Drawing.Icon ico = System.Drawing.Icon.FromHandle(hIcon);
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

	private static Result<IntPtr, E_SHGIL> GetIconPointer(int imagePtr, params ThumbnailSize[] sizes)
	{
		IImageList? spiml = null;
		Guid guil = new Guid(IID_IImageList);

		int hResult = -1;
		for (int i = 0; i < sizes.Length; i++)
		{
			hResult = Shell32.SHGetImageList((int)sizes[i], ref guil, ref spiml);
		
			if (hResult == 0)
				break;
			if (i <= sizes.Length - 1)
				LoggingHandler.Warn(nameof(GetIconPointer), "Couldn't get image for size: " + sizes[i] + " with hResult: " + hResult + "Scaling down to: "+ sizes[i - 1]);
		}
		
		if (hResult != 0)
			return new Result<IntPtr, E_SHGIL>(hResult);
		
		IntPtr hIcon = IntPtr.Zero;
		spiml!.GetIcon(imagePtr, Shell32.ILD_TRANSPARENT | Shell32.ILD_IMAGE, ref hIcon);

		return new Result<IntPtr, E_SHGIL>(hIcon);
	}

	[DllImport("Shell32.dll", EntryPoint = "ExtractIcon")]
    static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
}