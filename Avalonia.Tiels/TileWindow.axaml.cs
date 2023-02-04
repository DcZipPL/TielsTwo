using System;
using System.IO;
using System.Numerics;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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

		MainGrid.Background = new SolidColorBrush(App.Instance.Config.Tiles[ID].IsOverriden
			? App.Instance.Config.Tiles[ID].Color
			: App.Instance.Config.GlobalColor);
		EditBar.Background = new SolidColorBrush((Color)EditBarColor());
		if (App.Instance.Config.Tiles[ID].IsOverriden
			    ? App.Instance.Config.Tiles[ID].Theme == FluentThemeMode.Dark
			    : App.Instance.Config.GlobalTheme == FluentThemeMode.Dark)
		{
			TileName.Classes.Add("Dark");
			HideBtn.Classes.Add("Dark");
			OpenDirBtn.Classes.Add("Dark");
			RotateBtn.Classes.Add("Dark");
			RenameBtn.Classes.Add("Dark");
		}

		this.TransparencyLevelHint = App.Instance.Config.Tiles[ID].IsOverriden
			? App.Instance.Config.Tiles[ID].TransparencyLevel
			: App.Instance.Config.GlobalTransparencyLevel;

		var size = App.Instance.Config.Tiles[ID].Size;
		this.Width = size.X; this.Height = size.Y;
		
		var location = App.Instance.Config.Tiles[ID].Location;
		this.Position = new PixelPoint((int)location.X, (int)location.Y);

		this.TileName.Text = App.Instance.Config.Tiles[ID].Name;
	}

	public Color EditBarColor() => Color.Parse(App.Instance.Config.Tiles[ID].IsOverriden
			? App.Instance.Config.Tiles[ID].Theme == FluentThemeMode.Dark
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

	// Window hints (Resize, move, etc.)
	#region Window Resize

	private bool _isResizing;
	private Vector2 _position;
	
	private void ResizeDown(object? sender, PointerPressedEventArgs e)
	{
		_isResizing = true;
		_position = new Vector2((float)this.Width, (float)this.Height);
	}

	private void ResizeUp(object? sender, PointerReleasedEventArgs e)
	{
		_isResizing = false;
	}

	private void ResizeMove(object? sender, PointerEventArgs e)
	{
		if (!_isResizing) return;
		if (sender == null) return;
		var gridName = ((Grid)sender).Name!;
		var snapping = App.Instance.Config.Snapping;
		
		if (gridName.Contains('X'))
			this.Width = MathF.Ceiling((float)e.GetPosition(this).X / snapping) * snapping;
		if (gridName.Contains('Y'))
			this.Height = MathF.Ceiling((float)e.GetPosition(this).Y / snapping) * snapping;
		if (gridName.Contains('U'))
		{
			this.Width = MathF.Ceiling((float)e.GetPosition(this).X / snapping) * snapping + _position.X;
			//this.Position = new PixelPoint(this.Position.X - (int)(MathF.Ceiling((float)e.GetPosition(this).X / snapping) * snapping), this.Position.Y);
		}
		if (gridName.Contains('V'))
		{
			//this.Position = new PixelPoint(this.Position.X, this.Position.Y + (int)(MathF.Ceiling((float)e.GetPosition(this).Y / snapping) * snapping));
			this.Height = MathF.Ceiling((float)e.GetPosition(this).Y / snapping) * snapping + _position.Y;
		}

		System.Diagnostics.Debug.WriteLine($"[{DateTime.Now.Second}] X: {e.GetPosition(this).X} X: {e.GetPosition(this).Y}");
	}

	#endregion
}