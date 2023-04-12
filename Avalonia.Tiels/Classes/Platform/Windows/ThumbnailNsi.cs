using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;

using static Avalonia.Tiels.Classes.Platform.Windows.ThumbnailShellNsi;

namespace Avalonia.Tiels.Classes.Platform.Windows;

public class ThumbnailNsi : ThumbnailCsi
{
	protected override Bitmap GetThumbnailBitmap(string path, ThumbnailSize size)
	{
		IntPtr hIcon = GetIconPointer(GetIconIndex(path), size);
		
		using System.Drawing.Icon ico = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
		var bitmap = ico.ToBitmap().ConvertToAvaloniaBitmap();
		var hResult = Shell32.DestroyIcon(hIcon);
		if (hResult == 0)
			throw LoggingHandler.Error(new Exception("Failed to extract icon"), nameof(GetThumbnailBitmap));

		return bitmap;
	}


	protected override Bitmap GetDirectoryBitmap(int offest = 3)
	{
		string fileName = @$"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\imageres.dll"; // path to imageres.dll
		IntPtr hIcon = ExtractIcon(IntPtr.Zero, fileName, offest); // get icon from imageres.dll

		if (hIcon != IntPtr.Zero)
		{
			using System.Drawing.Icon ico = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
			var bitmap = ico.ToBitmap().ConvertToAvaloniaBitmap();
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
			(uint)System.Runtime.InteropServices.Marshal.SizeOf(sfi),
			(uint)(SHGFI.SysIconIndex | SHGFI.LargeIcon | SHGFI.UseFileAttributes));
		return sfi.iIcon;
	}

	private static IntPtr GetIconPointer(int imagePtr, ThumbnailSize size)
	{
		IImageList spiml = null;
		Guid guil = new Guid(IID_IImageList);

		Shell32.SHGetImageList((int)size, ref guil, ref spiml);
		IntPtr hIcon = IntPtr.Zero;
		spiml.GetIcon(imagePtr, Shell32.ILD_TRANSPARENT | Shell32.ILD_IMAGE, ref hIcon);

		return hIcon;
	}

	[DllImport("Shell32.dll", EntryPoint = "ExtractIcon")]
    static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);
}