using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;
using Avalonia.Tiels.Controls;

namespace Avalonia.Tiels;

public partial class TileWindow : Window
{
	public const float CELL_WIDTH = 120;
	public const float CELL_HEIGHT = 80;
	
	public record TileEntry(string Path, IImage Preview);
	public Dictionary<string, TileEntry> entries = new();

	private bool _editMode = false;
	private bool _isHidden = false;
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

		float handleHeight = App.Instance.Config.HandleHeight;
		HandleSizeDefinition.RowDefinitions[1].Height = new GridLength(handleHeight);
		MoveArea.Margin = new Thickness(0, 0, 0, handleHeight);

		MainGrid.Background = new SolidColorBrush(App.Instance.Config.Tiles[ID].IsOverriden
			? App.Instance.Config.Tiles[ID].Color
			: App.Instance.Config.GlobalColor);
		EditBar.Background = new SolidColorBrush((Color)EditBarColor());
		if (App.Instance.Config.Tiles[ID].IsOverriden
			    ? App.Instance.Config.Tiles[ID].Theme == FluentThemeMode.Dark
			    : App.Instance.Config.GlobalTheme == FluentThemeMode.Dark)
		{
			MoveAreaText.Classes.Add("Dark");
			TileName.Classes.Add("Dark");
			HideBtn.Classes.Add("Dark");
			OpenDirBtn.Classes.Add("Dark");
			RotateBtn.Classes.Add("Dark");
			RenameBtn.Classes.Add("Dark");
		}

		this.TransparencyLevelHint = App.Instance.Config.Tiles[ID].IsOverriden
			? App.Instance.Config.Tiles[ID].TransparencyLevel
			: App.Instance.Config.GlobalTransparencyLevel;

		var location = App.Instance.Config.Tiles[ID].Location;
		this.Position = new PixelPoint((int)location.X, (int)location.Y);

		this.TileName.Text = App.Instance.Config.Tiles[ID].Name;

		_isHidden = App.Instance.Config.Tiles[ID].Hidden;

		var size = App.Instance.Config.Tiles[ID].Size;
		this.Width = size.X;
		
		UpdateWindowHiddenState();
	}
	
	private void UpdateWindowHiddenState()
	{
		var size = App.Instance.Config.Tiles[ID].Size;

		if (_isHidden)
		{
			HideBtn.Content = Icons.ChevronDown;
			EntryContent.IsVisible = false;
			this.Height = App.Instance.Config.HandleHeight;
		}
		else
		{
			HideBtn.Content = Icons.ChevronUp;
			EntryContent.IsVisible = true;
			this.Height = size.Y;
		}
	}

	public void ReorderEntries()
	{
		EntryContent.Children.Clear();
		// TODO: Add ordering modes

		// Spawn entries
		foreach (var e in entries)
		{
			var extension = Path.GetExtension(e.Value.Path);
			var entry = new EntryComponent
			{
				Path = e.Value.Path,
				EntryName = Path.GetFileNameWithoutExtension(e.Value.Path) + (extension is ".url" or ".lnk" ? "" : extension),
				Preview = e.Value.Preview,
				Theme = App.Instance.Config.Tiles[ID].IsOverriden
					? App.Instance.Config.Tiles[ID].Theme
					: App.Instance.Config.GlobalTheme,
				Attribute = extension is ".url" or ".lnk"
					? FileAttribute.Link
					: FileAttribute.Normal
			};
			entry.Width = CELL_WIDTH;
			entry.Height = CELL_HEIGHT;
			this.EntryContent.Children.Add(entry);
		}
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
		// Handle Tile content
		var loadContentThread = new Thread(() => TileManagement.LoadTileContent(this, App.Instance.Config));
		loadContentThread.Start();
	}
	
	// Buttons
	private void ToggleEditMode(object? sender, RoutedEventArgs e)
	{
		// Save changes
		if (_editMode)
		{
			App.Instance.Config.Tiles[ID].Size = new Vector2((float)this.Width, (float)this.Height);
			App.Instance.Config.Tiles[ID].Location = new Vector2((float)this.Position.X, (float)this.Position.Y);
			ReorderEntries();
		}
		
		_editMode = !_editMode;
		WindowHints.IsVisible = _editMode;
		RenameBtn.Background = new SolidColorBrush(Color.Parse(_editMode ? "#25000000" : "#00000000"));
	}
	
	private void ToggleHideMode(object? sender, RoutedEventArgs e)
	{
		_isHidden = !_isHidden;
		App.Instance.Config.Tiles[ID].Hidden = _isHidden;
		UpdateWindowHiddenState();
	}
	
	// Window hints (Resize, move, etc.)
	#region Window Resize

	private bool _isResizing;
	private bool _isMoving;
	private Vector2 _originPosition;
	private Vector2? _originMousePosition;
	
	private void ResizeDown(object? sender, PointerPressedEventArgs e)
	{
		if (_editMode)
		{
			_isResizing = true;
			_originPosition = new Vector2((float)this.Width, (float)this.Height);
		}
	}

	private void ResizeUp(object? sender, PointerReleasedEventArgs e)
	{
		_isResizing = false;
	}

	private void ResizeMove(object? sender, PointerEventArgs e)
	{
		if (!_editMode) { _isResizing = false; return; }
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
			// TODO: impl win-like resize
			this.Width = MathF.Ceiling((float)e.GetPosition(this).X / snapping) * snapping + _originPosition.X;
		}
		if (gridName.Contains('V'))
		{
			// TODO: impl win-like resize
			this.Height = MathF.Ceiling((float)e.GetPosition(this).Y / snapping) * snapping + _originPosition.Y;
		}
	}

	#endregion

	#region Window Movement
	private void MoveDown(object? sender, PointerPressedEventArgs e) => _isMoving = true;

	private void MoveUp(object? sender, PointerReleasedEventArgs e)
	{
		_originMousePosition = null;
		_isMoving = false;
	}

	private void Move(object? sender, PointerEventArgs e)
	{
		if (!_editMode) { _isResizing = false; return; }
		if (!_isMoving) return;
		var snapping = App.Instance.Config.Snapping;

		_originMousePosition ??= new Vector2((float)e.GetPosition(this).X, (float)e.GetPosition(this).Y);
		
		this.Position = new PixelPoint(
			this.Position.X +(int)(MathF.Ceiling(((float)e.GetPosition(this).X - (int)_originMousePosition.Value.X) / snapping) * snapping), 
			this.Position.Y + (int)(MathF.Ceiling(((float)e.GetPosition(this).Y - (int)_originMousePosition.Value.Y) / snapping) * snapping)
		);
	}
	#endregion

	private void OpenContentDirectory()
	{
		var path = App.Instance.Config.Tiles[ID].Path;
		if (path is null) return;
		if (!Directory.Exists(path)) return;
		Process.Start(new ProcessStartInfo
		{
			FileName = path,
			UseShellExecute = true,
			Verb = "open"
		});
	}
	
	private void OpenContentDirectory(object? sender, RoutedEventArgs e) => OpenContentDirectory();
}