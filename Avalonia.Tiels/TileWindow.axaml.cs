using System;
using System.IO;
using System.Numerics;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels;

public partial class TileWindow : Window
{
	public Guid ID { get; set; }
	
	public TileWindow()
	{
		InitializeComponent();
		#if DEBUG
			this.AttachDevTools();
		#endif
		EditBar.Background = new SolidColorBrush((Color)EditBarColor());
	}

	public Color EditBarColor()
	{
		return Color.Parse(App.Instance.Config.GlobalTheme == FluentThemeMode.Dark ? "#25000000" : "#25ffffff");
	}

	private void OnLoad(object? sender, EventArgs e)
	{
		var loadTileThread = new Thread(() => this.LoadTile(App.Instance.Config, ID));
		loadTileThread.Start();
	}

	public void LoadTile(Configuration configuration, Guid id)
	{
		if (!configuration.Tile.TileExist(id))
			Configuration.CreateTileConfig(id);
	}
}