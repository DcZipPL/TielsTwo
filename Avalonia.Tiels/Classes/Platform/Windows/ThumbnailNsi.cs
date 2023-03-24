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
		return bitmap;
		
		throw new System.NotImplementedException();
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
}