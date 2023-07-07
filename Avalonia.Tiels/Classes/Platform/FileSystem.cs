using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Avalonia.Tiels.Classes.Platform;

public class FileSystem
{
	public static void SendFileToTrash(string path)
	{
		if (OperatingSystem.IsWindows())
		{
			Windows.SendFileToRecycleBin(path);
		}
	}
	
	private static class Windows
	{
		public static void SendFileToRecycleBin(string filePath)
		{
			if (!File.Exists(filePath) && !Directory.Exists(filePath))
			{
				throw new FileNotFoundException("File not found.", filePath);
			}

			SHFILEOPSTRUCT fileOperation = new SHFILEOPSTRUCT();
			fileOperation.wFunc = FO_DELETE;
			fileOperation.pFrom = filePath + '\0'; // Multiple files can be specified using a null-delimited string
			fileOperation.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;

			int result = SHFileOperation(ref fileOperation);
			if (result != 0)
			{
				throw new IOException("Failed to move the file to the recycle bin.", result);
			}
		}

		// Constants and structures required for the SHFileOperation function
		private const int FO_DELETE = 0x0003;
		private const int FOF_ALLOWUNDO = 0x0040;
		private const int FOF_NOCONFIRMATION = 0x0010;

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct SHFILEOPSTRUCT
		{
			public IntPtr hwnd;
			public int wFunc;
			public string pFrom;
			public string pTo;
			public short fFlags;
			public bool fAnyOperationsAborted;
			public IntPtr hNameMappings;
			public string lpszProgressTitle;
		}
	}
}