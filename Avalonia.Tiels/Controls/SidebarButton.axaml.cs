using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Avalonia.Tiels.Controls;

public partial class SidebarButton : UserControl
{
	public string Text { get; set; }
	public FontFamily Font { get; set; }
	public string Icon { get; set; }
	
	public SidebarButton()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}