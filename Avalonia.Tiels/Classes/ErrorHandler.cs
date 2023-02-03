using System;
using System.IO;
using System.Text;
using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.Tiels;

public class ErrorHandler
{
	public const string ERROR_MODE = "error";

	public static Exception ShowErrorWindow(Exception e, byte hex)
	{
		System.Diagnostics.Process.Start("../Tiels" + (OperatingSystem.IsWindows() ? ".exe" : ""),
			ErrorHandler.ERROR_MODE +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes(App.I18n.GetString("WindowTitleError") + e.Message)) +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes(hex + " " + e)));
		return e;
	}

	public static void Warn(string source, string message)
	{
		Console.WriteLine("WARN: " + message);
	}
}