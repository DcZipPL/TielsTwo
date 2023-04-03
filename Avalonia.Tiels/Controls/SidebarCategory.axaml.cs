using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;

namespace Avalonia.Tiels.Controls;

public partial class SidebarCategory : UserControl
{
	public string Title { get; set; } = "";

	public SidebarCategory()
	{
		InitializeComponent();
	}
}