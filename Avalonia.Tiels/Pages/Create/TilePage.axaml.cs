using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Pages.Create;

public partial class TilePage : UserPage
{
	private TileType _type = TileType.Tile;
	
	private const double DEFAULT_WIDTH = 250;
	private const double DEFAULT_HEIGHT = 100;
	
	public TilePage SetPageAsDirectoryPortal()
	{
		_type = TileType.DirectoryPortal;
		TilePathPanel.IsVisible = true;
		return this;
	}
	
	public TilePage()
	{
		InitializeComponent();
		SizeXBox.Watermark = DEFAULT_WIDTH.ToString();
		SizeYBox.Watermark = DEFAULT_HEIGHT.ToString();

		PathBox.Text = Configuration.GetDefaultTilesDirectory();
	}

	public void CreateTile()
	{
		try
		{
			if (!Appearance.UseGlobalThemeBox.IsChecked.HasValue)
				throw LoggingHandler.Error(new NullReferenceException("UseGlobalThemeBox.IsChecked is null!"),
					nameof(TilePage));
			TileManagement.CreateTile(
				NameBox.Text,
				_type == TileType.DirectoryPortal ? PathBox.Text : Path.Combine(App.Instance.Config.TilesPath, NameBox.Text),
				SizeXBox.Text != null ? double.Parse(SizeXBox.Text) : DEFAULT_WIDTH,
				SizeYBox.Text != null ? double.Parse(SizeYBox.Text) : DEFAULT_HEIGHT,
				!Appearance.UseGlobalThemeBox.IsChecked.Value,
				(FluentThemeMode)Appearance.ThemeBox.SelectedIndex,
				(WindowTransparencyLevel)Appearance.TransparencyModeBox.SelectedIndex,
				Appearance.ColorBtn.Color
			);
		}
		catch (FormatException ex)
		{
			LoggingHandler.Warn("CreateTile", ex.ToString());
		}
	}
}