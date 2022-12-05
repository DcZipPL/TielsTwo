using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Tiels.Pages;

namespace Avalonia.Tiels.Controls;

public partial class SidebarButton : UserControl
{
	public string Text { get; set; }
	public FontFamily Font { get; set; }
	public string Icon { get; set; }
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