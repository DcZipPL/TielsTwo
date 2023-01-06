using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Svg;
using Avalonia.Tiels.Pages;

namespace Avalonia.Tiels.Controls;

public partial class SidebarButton : UserControl
{
	public string Text { get; set; }
	public string Icon { get; set; }
	public UserPage? Page { get; set; }
	
	public SidebarButton WithPage(UserPage? page)
	{
		this.Page = page;
		Util.SetSvgImage(Icon, SvgIcon);
		return this;
	}

	public SidebarButton()
	{
		InitializeComponent();
	}
}