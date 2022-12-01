using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;

namespace Avalonia.Tiels
{
	public partial class SettingsWindow : Window
	{
		private App app = (App)Application.Current!;
		private (string, string?) status = ("", App.I18n.GetString("NoChanges"));
		
		public SettingsWindow()
		{
			InitializeComponent();
		}
		
		private void ApplySettings()
		{
			// Check if tiles path didn't changed. If changed apply new location if valid to config and give status.
			var newPath = string.IsNullOrEmpty(TilesDirectoryBox.Text)
				? Configuration.GetDefaultTilesDirectory()
				: TilesDirectoryBox.Text;
				
			if (app.Config.TilesPath != newPath)
				if (Directory.Exists(newPath))
				{
					app.Config.TilesPath = TilesDirectoryBox.Text;
					ChangeSettingsStatus(App.I18n.GetString("ChangesAccepted")!, false);
				}
				else
					ChangeSettingsStatus(App.I18n.GetString("PathDontExist")!, true);
			else
				ChangeSettingsStatus(App.I18n.GetString("NoChanges")!, false);
			
			// Apply selections to config
			ApplyIfChanged(app.Config.Autostart,             b => app.Config.Autostart = b,             AutostartCheckBox);
			ApplyIfChanged(app.Config.AutostartHideSettings, b => app.Config.AutostartHideSettings = b, HideWindowCheckBox);
			ApplyIfChanged(app.Config.SpecialEffects,        b => app.Config.SpecialEffects = b,        EffectsCheckBox);
			ApplyIfChanged(app.Config.Experimental,          b => app.Config.Experimental = b,          ExperimentalCheckBox);
			
			// Save draft data before reinitialize
			var draftTilesPath = TilesDirectoryBox.Text;
			
			// Reinitialize everything if language is changed
			InitializeComponent();
			WindowOpened();
			
			// Load draft data
			TilesDirectoryBox.Text = draftTilesPath;
			
			// Show status
			StatusIcon.Text = status.Item1;
			StatusText.Text = status.Item2;
		}

		private void ChangeSettingsStatus(string message, bool invalid)
		{
			status = (invalid ? "" : "", message);
		}

		private void RollbackSettings()
		{
			
		}

		private void WindowOpened()
		{
			TilesDirectoryBox.Text = app.Config.TilesPath;
			TilesDirectoryBox.Watermark = Configuration.GetDefaultTilesDirectory();
			
			LanguageBox.Items = Util.ImplementedCultures();
			LanguageBox.SelectedItem = ((IEnumerable<CultureInfo>)LanguageBox.Items).ToList()
				.FirstOrDefault(item => Equals(item, CultureInfo.CurrentUICulture),
					((IEnumerable<CultureInfo>)LanguageBox.Items).ToList()[0]);
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
		
		private void LanguageChanged(object? sender, SelectionChangedEventArgs e)
		{
			try
			{
				CultureInfo ci = (CultureInfo)LanguageBox.SelectedItem;
				CultureInfo.CurrentCulture = ci;
				CultureInfo.CurrentUICulture = ci;
			}
			catch (Exception ex)
			{
				ErrorHandler.ShowErrorWindow(ex,"~(0x0003)");
				throw;
			}
		}
		
		private void SettingsOpened(object? sender, EventArgs e) => WindowOpened();
		private void ApplyButtonClicked(object? sender, RoutedEventArgs e) => ApplySettings();
		private void DefaultButtonClicked(object? sender, RoutedEventArgs e) => RollbackSettings();
	}
}