using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels.Pages.Settings;

public partial class UpdatesPage : UserPage
{
	public UpdatesPage()
	{
		InitializeComponent();
	}

	private void OpenDownloadPage(object? sender, RoutedEventArgs e)
	{
		var psi = new ProcessStartInfo
		{
			FileName = "https://github.com/DcZipPL/TielsTwo",
			UseShellExecute = true
		};
		Process.Start(psi);
	}
}