using System;
using System.IO;
using System.Text;
using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.Tiels;

public class ErrorHandler
{
	public const string ERROR_MODE = "print_error";

	public static void ShowErrorWindow(Exception e, string hex)
	{
		System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly()
				.Location.Replace(".dll",OperatingSystem.IsWindows() ? ".exe" : ""),
			ErrorHandler.ERROR_MODE +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes(App.I18n.GetString("WindowTitleError") + e.Message)) +
			" " +
			Convert.ToBase64String(Encoding.UTF8.GetBytes(hex + " " + e)));
	}
}