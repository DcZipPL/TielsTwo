using Avalonia.Controls;

namespace Avalonia.Tiels.Controls;

public partial class SidebarCategory : UserControl
{
	public string Title { get; set; } = "";

	public SidebarCategory()
	{
		InitializeComponent();
	}
}