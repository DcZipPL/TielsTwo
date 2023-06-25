using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Controls;

public partial class TileManageListEntry : UserControl
{
	public TileType Type { get; set; } = TileType.Tile;
	public string Text { get; set; } = "";
	public TileManageListEntry()
	{
		InitializeComponent();
	}
}