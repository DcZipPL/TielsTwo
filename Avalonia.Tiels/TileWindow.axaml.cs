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
		if (App.Instance.Config.Tiles[ID].IsOverriden
			    ? App.Instance.Config.Tiles[ID].GlobalTheme == FluentThemeMode.Dark
			    : App.Instance.Config.GlobalTheme == FluentThemeMode.Dark)
		{
			TileName.Classes.Add("Dark");
			HideBtn.Classes.Add("Dark");
			OpenDirBtn.Classes.Add("Dark");
			RotateBtn.Classes.Add("Dark");
			RenameBtn.Classes.Add("Dark");
		}

		var size = App.Instance.Config.Tiles[ID].Size;
		this.Width = size.X; this.Height = size.Y;
		
		var location = App.Instance.Config.Tiles[ID].Location;
		this.Position = new PixelPoint((int)location.X, (int)location.Y);

		this.TileName.Text = App.Instance.Config.Tiles[ID].Name;
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
		var loadContentThread = new Thread(() => TileManagement.LoadTileContent(this, App.Instance.Config));
		loadContentThread.Start();
	}
}