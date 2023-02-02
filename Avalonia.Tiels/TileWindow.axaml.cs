using System;
using System.IO;
using System.Numerics;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.Tiels.Classes;
using Avalonia.Tiels.Controls;

namespace Avalonia.Tiels;

public partial class TileWindow : Window
{
	public Guid ID { get; }
	
	public TileWindow()
	{
		InitializeComponent();
		#if DEBUG
			this.AttachDevTools();
		#endif
	}

	public TileWindow(Guid id) : this()
	{
		ID = id;
		EditBar.Background = new SolidColorBrush((Color)EditBarColor());
		var size = App.Instance.Config.Tiles[ID].Size;
		this.Width = size.X; this.Height = size.Y;
		
		var location = App.Instance.Config.Tiles[ID].Location;
		this.Position = new PixelPoint((int)location.X, (int)location.Y);
	}

	public Color EditBarColor() => Color.Parse(App.Instance.Config.Tiles[ID].IsOverriden
			? App.Instance.Config.Tiles[ID].GlobalTheme == FluentThemeMode.Dark
				? "#25000000"
				: "#25ffffff"
			: App.Instance.Config.GlobalTheme == FluentThemeMode.Dark
				? "#25000000"
				: "#25ffffff");

	private void OnLoad(object? sender, EventArgs e)
	{
		var loadContentThread = new Thread(() => this.LoadContent(App.Instance.Config, ID));
		loadContentThread.Start();
	}

	public void LoadContent(Configuration configuration, Guid id)
	{
		foreach (var systemEntry in Directory.EnumerateFileSystemEntries(configuration.Tiles[id].Path))
		{
			// TODO: Better threading if possible
			Dispatcher.UIThread.Post(() =>
			{
				// TODO: Get thumbnails from os
				var thumbnail = Util.SetSvgImage("/Assets/Icons/out/alert-octagon.svg", new Image());
				var entry = new EntryComponent
				{
					EntryName = Path.GetFileName(systemEntry),
					Preview = thumbnail
				};
				EntryContent.Children.Add(entry);
			});
		}
	}
}