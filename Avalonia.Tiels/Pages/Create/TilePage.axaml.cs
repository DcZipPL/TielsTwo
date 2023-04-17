using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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

		TransparencyModeBox.Items = Enum.GetNames(typeof(WindowTransparencyLevel)).Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"));
		TransparencyModeBox.SelectedIndex = (int)App.Instance.Config.GlobalTransparencyLevel;

		ThemeBox.Items = Enum.GetNames(typeof(FluentThemeMode));
		ThemeBox.SelectedIndex = (int)App.Instance.Config.GlobalTheme;

		UseGlobalThemeBox.Checked += (sender, args) => CustomAppearanceGrid.IsEnabled = false;
		UseGlobalThemeBox.Unchecked += (sender, args) => CustomAppearanceGrid.IsEnabled = true;

		ColorBtn.Color = App.Instance.Config.GlobalTheme == FluentThemeMode.Dark ? Util.TILE_DARK_COLOR : Util.TILE_LIGHT_COLOR;
		ThemeBox.SelectionChanged += (_, _) =>
		{
			if (ColorBtn.Color == Util.TILE_DARK_COLOR || ColorBtn.Color == Util.TILE_LIGHT_COLOR)
				ColorBtn.Color = (FluentThemeMode)ThemeBox.SelectedIndex == FluentThemeMode.Dark ? Util.TILE_DARK_COLOR: Util.TILE_LIGHT_COLOR;
		};
		
		TransparencyModeBox.SelectionChanged += (_, _) =>
		{
			var selection = (string?)TransparencyModeBox.SelectedItem ?? WindowTransparencyLevel.None.ToString();
			WarnUnsupportedOptionText.IsVisible = selection != WindowTransparencyLevel.None.ToString() &&
			                                      selection != WindowTransparencyLevel.Transparent.ToString();
			NewestWinOnlyText.IsVisible = selection == WindowTransparencyLevel.Mica.ToString();
			WarnOtherProgramsText.IsVisible = selection == WindowTransparencyLevel.Mica.ToString();
		};

		PathBox.Text = Configuration.GetDefaultTilesDirectory();
	}

	public void CreateTile()
	{
		try
		{
			if (!UseGlobalThemeBox.IsChecked.HasValue)
				throw LoggingHandler.Error(new NullReferenceException("UseGlobalThemeBox.IsChecked is null!"),
					nameof(TilePage));
			TileManagement.CreateTile(
				NameBox.Text,
				_type == TileType.DirectoryPortal ? PathBox.Text : Path.Combine(App.Instance.Config.TilesPath, NameBox.Text),
				SizeXBox.Text != null ? double.Parse(SizeXBox.Text) : DEFAULT_WIDTH,
				SizeYBox.Text != null ? double.Parse(SizeYBox.Text) : DEFAULT_HEIGHT,
				!UseGlobalThemeBox.IsChecked.Value,
				(FluentThemeMode)ThemeBox.SelectedIndex,
				(WindowTransparencyLevel)TransparencyModeBox.SelectedIndex,
				ColorBtn.Color
			);
		}
		catch (FormatException ex)
		{
			LoggingHandler.Warn("CreateTile", ex.ToString());
		}
	}
}