namespace Avalonia.Tiels.Classes;

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

public class Platform
{
    public static Avalonia.Media.Imaging.Bitmap GetThumbnailBitmap(Image image, string path)
    {
        //image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        using (var stream = new MemoryStream())
        {
            // Save the image to a memory stream in PNG format
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            // Reset the memory stream position
            stream.Seek(0, SeekOrigin.Begin);

            // Create a new Bitmap object from the memory stream
            return new Avalonia.Media.Imaging.Bitmap(stream);
        }
        throw new Exception("Could not create thumbnail bitmap.");
    }

    public static Image GetThumbnailImage(string filePath, int width, int height)
    {
        // Create a SHFILEINFO structure
        SHFILEINFO shinfo = new SHFILEINFO();

        // Get the file attributes
        uint flags = SHGFI_SYSICONINDEX | SHGFI_USEFILEATTRIBUTES | SHGFI_LARGEICON;
        SHGetFileInfo(filePath, 256 /* FILE_ATTRIBUTE_NORMAL */, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);

        // Get system image list containing the icon
        IntPtr hImageList;
        Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
        IntPtr himl = SHGetImageList(SHIL_LARGE, ref iidImageList, out hImageList);

        // Get the icon for the file
        IntPtr hIcon = ImageList_GetIcon(hImageList, shinfo.iIcon, ILD_TRANSPARENT);

        // Create a Bitmap from the icon
        Bitmap bitmap = Bitmap.FromHicon(hIcon);

        // Resize the bitmap to the desired dimensions
        Bitmap thumbnail = new Bitmap(bitmap, new Size(width, height));

        // Clean up resources
        DestroyIcon(hIcon);
        bitmap.Dispose();

        // Return the thumbnail image
        return thumbnail;
    }

    // Import the necessary Win32 API functions
    const uint SHGFI_SYSICONINDEX = 0x4000;
    const uint SHGFI_SMALLICON = 0x0001;
    const uint SHGFI_LARGEICON = 0x000000000;
    const uint SHGFI_LINKOVERLAY = 0x000008000;
    const uint SHGFI_ADDOVERLAYS = 0x000000020;
    const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
    const uint SHIL_LARGE = 0;
    const uint ILD_TRANSPARENT = 1;

    [DllImport("shell32.dll")]
    static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

    [DllImport("comctl32.dll", SetLastError = true)]
    static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

    [DllImport("shell32.dll")]
    static extern IntPtr SHGetImageList(uint iImageList, ref Guid riid, out IntPtr ppv);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool DestroyIcon(IntPtr hIcon);

    [StructLayout(LayoutKind.Sequential)]
    struct IMAGELIST
    {
    }
    [StructLayout(LayoutKind.Sequential)]
    struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }
}