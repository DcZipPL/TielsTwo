using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

	public record TileEntry(string Path, IImage Preview) { public bool IsAdded; }
	public List<TileEntry> entries = new();

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

	internal void LoadEntries(uint cursor)
	{
		// TODO: Add ordering modes
		EntryContent.Children.Clear();
		this.entries.ForEach(entry => entry.IsAdded = false);

		AddEntries(cursor);

		// Set EntryContent height for modern scroll
		this.EntryContent.Height = GetCell(this.entries.Count - 1).Item2 * CELL_HEIGHT + CELL_HEIGHT;
	}

	internal void AddEntries(uint cursor)
	{
		Test__Item.Text = cursor.ToString();
		uint limit = cursor + App.Instance.Config.EntryLimit;
		for (int i = (int) cursor; i < this.entries.Count; i++)
		{
			if (this.entries[i].IsAdded) continue;
			AddEntry(this.entries[i]);

			if (--limit == 0 && App.Instance.Config.EntryLimit != 0) break;
		}
	}

	private void AddEntry(TileEntry entryData)
	{
		var extension = Path.GetExtension(entryData.Path);
		var entry = new EntryComponent
		{
			Path = entryData.Path,
			EntryName = Path.GetFileNameWithoutExtension(entryData.Path) + (extension is ".url" or ".lnk" ? "" : extension),
			Preview = entryData.Preview,
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

		entryData.IsAdded = true;
	}

	private (int, int) GetCellAmount() =>
		((int)Math.Floor(this.Width / CELL_WIDTH), (int)Math.Floor(this.Height / CELL_HEIGHT));
	
	private (int, int) GetCell(int index)
	{
		var cells = GetCellAmount();
		return (index % cells.Item1, (int)Math.Floor((double)index / (double)cells.Item1));
	}

	private Color EditBarColor() => Color.Parse(App.Instance.Config.Tiles[ID].IsOverriden
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
			LoadEntries(0);
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
	private void ScrollEntryLoad(object? sender, ScrollChangedEventArgs e) => AddEntries((uint)(((ScrollViewer)sender!).Offset.Y / CELL_HEIGHT));
}