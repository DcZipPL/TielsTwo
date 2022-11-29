using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;

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
			
		}

		private void ApplyButtonClicked(object? sender, RoutedEventArgs e) => ApplySettings();
	}
}