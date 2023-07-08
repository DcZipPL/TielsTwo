using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Controls;

public partial class TileManageListEntry : UserControl
{
	public TileType Type { get; set; } = TileType.Tile;
	public string Text { get; set; } = "";
	public Guid ID { get; set; } = Guid.Empty;
	
	public TileManageListEntry()
	{
		InitializeComponent();
	}
	
	private void DestroyTile(object? sender, RoutedEventArgs e)
	{
		TileManagement.DeleteTile(ID);
		this.IsVisible = false;
	}
}