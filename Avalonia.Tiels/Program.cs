using Avalonia;
using System;
using Avalonia.Controls;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels
{
	class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args)
		{
			try
			{
				BuildAvaloniaApp()
					.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
			} catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Application panic! FATAL: " + e);
				ErrorHandler.Fatal(e, "[panic!]");
			}
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToTrace();
	}
}