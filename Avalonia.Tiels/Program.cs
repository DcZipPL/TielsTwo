using Avalonia;
using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Tiels.Classes;
using Serilog;

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
			// Initialize Serilog
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File("./logs/adt.log", rollingInterval: RollingInterval.Day)
				.CreateLogger();
			
			try
			{
				if (!Directory.Exists("./logs"))
					Directory.CreateDirectory("./logs");
				
				// Start application
				BuildAvaloniaApp()
					.StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
				
			} catch (Exception e)
			{
				// Panic! Log the error and exit.
				LoggingHandler.Fatal(e, "[panic!]");
			}
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToTrace();
	}
}