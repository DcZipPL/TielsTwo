using System;
using System.Text;
using Serilog;

namespace Avalonia.Tiels.Classes;

public static class LoggingHandler
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
		var err = ShowErrorWindow(source, ex, Level.FATAL);
		Log.Fatal($"(@{source}) Application panic!\nFATAL: {ex}");
		return err;
	}
	
	public static Exception Error(Exception ex, string source)
	{
		Log.Error($"@{source} > {ex}");
		return ShowErrorWindow(source, ex, Level.ERROR);
	}
	
	public static void Warn(string source, string message)
	{
		Log.Warning($"@{source} > {message}");
	}
	
	public enum Level
	{
		ERROR,
		FATAL
	}
}