using System;
using System.Text;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels
{
	public partial class App : Application
	{
		public Configuration Config { get; set; }
		
		public SettingsWindow? ActiveSettingsWindow;
		
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			var args = Environment.GetCommandLineArgs();

			if (args.Length <= 1 || args[1] != ErrorHandler.ERROR_MODE)
			{
				if (ApplicationLifetime is IControlledApplicationLifetime desktop)
				{
					ExecuteApplication(desktop);
				}
			}
			else
			{
				if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime eDesktop)
				{
					var msgWindow = new MessageWindow();
					msgWindow.WindowTitle = Encoding.UTF8.GetString(Convert.FromBase64String(args[2]));
					msgWindow.Message = Encoding.UTF8.GetString(Convert.FromBase64String(args[3]));
					eDesktop.MainWindow = msgWindow;
				}
			}

			base.OnFrameworkInitializationCompleted();
		}

		private void ExecuteApplication(IControlledApplicationLifetime desktop)
		{
			if (Configuration.IsFirstStartup())
			{
				Config = Configuration.Init(desktop);
			}
			else
			{
				// Load tiles
				Config = Configuration.Load(desktop);
			}
			
			if (!Config.AutostartHideSettings || Configuration.IsFirstStartup())
			{
				ActiveSettingsWindow = new SettingsWindow();
				ActiveSettingsWindow.Show();
			}
		}
	}
}