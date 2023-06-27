using Avalonia;
using Avalonia.Controls;
using Avalonia.Tiels.Pages;

namespace Avalonia.Tiels.Controls;

public partial class SidebarButton : UserControl
{
	public bool IsToggled { get; set; }
	public string Text { get; set; } = "";
	public string Icon { get; set; } = "";
	public string Glyph => Icon;
	public UserPage? Page { get; set; }

	public SidebarButton WithPage(UserPage? page)
	{
		this.Page = page;
		return this;
	}

	public SidebarButton()
	{
		InitializeComponent();
	}
}