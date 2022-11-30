using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;

namespace Avalonia.Tiels
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}
		
		private void ApplySettings()
		{
			var current = (App)Application.Current!;

			var newPath = string.IsNullOrEmpty(TilesDirectoryBox.Text)
				? Configuration.GetDefaultTilesDirectory()
				: TilesDirectoryBox.Text;
				
			if (current.Config.TilesPath != newPath)
				if (Directory.Exists(TilesDirectoryBox.Text))
				{
					current.Config.TilesPath = TilesDirectoryBox.Text;
					ChangeSettingsStatus(App.I18n.GetString("ChangesAccepted")!, true);
				}
				else
					ChangeSettingsStatus(App.I18n.GetString("PathDontExist")!, true);
			ApplyIfChanged(current.Config.Autostart,             b => current.Config.Autostart = b,             AutostartCheckBox);
			ApplyIfChanged(current.Config.AutostartHideSettings, b => current.Config.AutostartHideSettings = b, HideWindowCheckBox);
			ApplyIfChanged(current.Config.SpecialEffects,        b => current.Config.SpecialEffects = b,        EffectsCheckBox);
			ApplyIfChanged(current.Config.Experimental,          b => current.Config.Experimental = b,          ExperimentalCheckBox);
		}

		private void ChangeSettingsStatus(string message, bool invalid)
		{
			StatusText.Text = message;
			StatusIcon.Text = invalid ? "" : "";
		}

		private void RollbackSettings()
		{
			
		}
		
		private void SettingsOpened(object? sender, EventArgs e)
		{
			TilesDirectoryBox.Watermark = Configuration.GetDefaultTilesDirectory();
		}

		private void TilePathTextBoxChanged(object? sender, KeyEventArgs e)
		{
			if (!Directory.Exists(TilesDirectoryBox.Text))
			{
				TilesDirectoryBox.BorderBrush = new SolidColorBrush(0xFFFF0000);
				TilesDirectoryBox.Foreground = new SolidColorBrush(0xFFFF0000);
			}
			else
			{
				// TODO: Use resources and not hardcoded values
				TilesDirectoryBox.BorderBrush = new SolidColorBrush(0xFF2A2A2A);
				TilesDirectoryBox.Foreground = new SolidColorBrush(0xFF000000);
			}
		}

		private void ApplyIfChanged(bool getter, Action<bool> setter, CheckBox checkBox)
		{
			if (getter != checkBox.IsChecked && checkBox.IsChecked != null)
				setter(checkBox.IsChecked!.Value);
		}

		private void ApplyButtonClicked(object? sender, RoutedEventArgs e) => ApplySettings();
		private void DefaultButtonClicked(object? sender, RoutedEventArgs e) => RollbackSettings();
	}
}