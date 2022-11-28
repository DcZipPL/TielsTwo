using System;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels;

public partial class TileWindow : Window, Tile
{
	public TileWindow()
	{
		InitializeComponent();
		#if DEBUG
			this.AttachDevTools();
		#endif
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	private void OnLoad(object? sender, EventArgs e)
	{
		
	}

	public string TypeName => "FileTile";
	public Vector2 Size { get; set; }
}