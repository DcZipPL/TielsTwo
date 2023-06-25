using System;
using System.Collections.Generic;
using System.IO;
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
		
		public List<TileWindow> ActiveTileWindows = new();
		
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
			
			Instance = (App)Application.Current!;
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IControlledApplicationLifetime desktop)
			{
				ExecuteApplication(desktop);
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
				Config = Configuration.Load(desktop);
				foreach (var tile in Config.Tiles)
				{
					if (Directory.Exists(tile.Value.Path))
						ActiveTileWindows.Add(TileManagement.LoadTile(tile.Key));
					// TODO: Inform user that tile directory does not exist.
				}
			}
			
			if (!Config.AutoStartHideSettings || Configuration.IsFirstStartup())
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