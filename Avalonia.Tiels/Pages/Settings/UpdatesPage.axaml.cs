using System.Diagnostics;
using Avalonia.Interactivity;

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