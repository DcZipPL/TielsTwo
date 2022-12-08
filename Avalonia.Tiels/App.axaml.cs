using System;
using System.Resources;
using System.Text;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels
{
	public partial class App : Application
	{
		public static App Instance;
		
		public static readonly ResourceManager I18n = Avalonia.Tiels.Resources.Resources.ResourceManager;
		public Configuration Config { get; set; }

		public SettingsWindow? ActiveSettingsWindow;
		
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
			
			Instance = (App)Application.Current!;
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
		
		private void TrayExitClick(object? sender, EventArgs e)
		{
			if (ApplicationLifetime is IControlledApplicationLifetime desktop)
			{
				desktop.Shutdown();
			}
		}

		private void TrayOpenSettings(object? sender, EventArgs e) => OpenSettings();
		private void OpenSettings()
		{
			if ((ActiveSettingsWindow == null || ActiveSettingsWindow.PlatformImpl != null) &&
			    ActiveSettingsWindow != null) return;
			ActiveSettingsWindow = new SettingsWindow();
			ActiveSettingsWindow.Show();
		}
	}
}