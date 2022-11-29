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
			
			var newPath = TilesDirectoryBox.Text == ""
				? Configuration.GetDefaultTilesDirectory()
				: TilesDirectoryBox.Text;
				
			if (current.Config.TilesPath != TilesDirectoryBox.Text)
				if (Directory.Exists(TilesDirectoryBox.Text))
						current.Config.TilesPath = TilesDirectoryBox.Text;

		}
		
		private void SettingsOpened(object? sender, EventArgs e)
		{
			TilesDirectoryBox.Watermark = Configuration.GetDefaultTilesDirectory();
		}
		
		private void ApplyButtonClicked(object? sender, RoutedEventArgs e) => ApplySettings();

		private void TilePathTextBoxChanged(object? sender, KeyEventArgs e)
		{
			if (!Directory.Exists(TilesDirectoryBox.Text))
			{
				TilesDirectoryBox.BorderBrush = new SolidColorBrush(0xFFFF0000);
				TilesDirectoryBox.Foreground = new SolidColorBrush(0xFFFF0000);
			}
			else
			{
				TilesDirectoryBox.BorderBrush = new SolidColorBrush(0xFF2A2A2A);
				TilesDirectoryBox.Foreground = new SolidColorBrush(0xFF000000);
			}
		}
	}
}