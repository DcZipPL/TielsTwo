using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Image = System.Drawing.Image;

namespace Avalonia.Tiels.Controls;

public partial class EntryComponent : UserControl
{
	public Image Preview { get; set; }
	public string EntryName { get; set; }
	
	public EntryComponent()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}