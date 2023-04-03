using System;
using System.Text;

namespace Avalonia.Tiels.Classes;

public class ErrorHandler
{
	public const string ERROR_MODE = "error";

	private static Exception ShowErrorWindow(string source, Exception e, Level level)
	{
		System.Diagnostics.Process.Start("../Tiels" + (OperatingSystem.IsWindows() ? ".exe" : ""),
			ERROR_MODE +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes(App.I18n.GetString("WindowTitleError") + e.Message)) +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes(e.ToString())) +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes(level.ToString())) +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes(source)) +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes("none")));
		return e;
	}

	public static Exception Fatal(Exception ex, string source)
	{
		System.Diagnostics.Debug.WriteLine("FATAL: " + ex);
		return ShowErrorWindow(source, ex, Level.FATAL);
	}
	
	public static Exception Error(Exception ex, string source)
	{
		System.Diagnostics.Debug.WriteLine("Error: " + ex);
		return ShowErrorWindow(source, ex, Level.ERROR);
	}
	
	public static void Warn(string source, string message)
	{
		System.Diagnostics.Debug.WriteLine("WARN: " + message);
	}
	
	public enum Level
	{
		ERROR,
		FATAL
	}
}