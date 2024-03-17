namespace Avalonia.Tiels.Classes.Image;

public class FileHeaders
{
	public static bool IsQoiImage(byte[] bytes)
	{
		if (bytes.Length < 4)
			return false;

		return bytes[0] == 0x71 && bytes[1] == 0x6f && bytes[2] == 0x69 && bytes[3] == 0x66;
	}
	
	public static bool IsPngImage(byte[] bytes)
	{
		if (bytes.Length < 8)
			return false;

		return bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
		       bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
	}
	
	public static bool IsJpegImage(byte[] bytes)
	{
		if (bytes.Length < 2)
			return false;

		return bytes[0] == 0xFF && bytes[1] == 0xD8;
	}
}