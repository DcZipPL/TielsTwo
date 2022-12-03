using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels.Pages.Settings;

public partial class AppearancePage : UserPage
{
	public AppearancePage()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}