using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Pages.Create;

public partial class TilePage : UserPage
{
	public TilePage()
	{
		InitializeComponent();

		TransparencyModeBox.Items = Enum.GetNames(typeof(WindowTransparencyLevel)).Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"));
		TransparencyModeBox.SelectedIndex = (int)App.Instance.Config.GlobalTransparencyLevel;

		ThemeBox.Items = Enum.GetNames(typeof(FluentThemeMode));
		ThemeBox.SelectedIndex = (int)App.Instance.Config.GlobalTheme;

		UseGlobalThemeBox.Checked += (sender, args) => CustomAppearanceGrid.IsEnabled = false;
		UseGlobalThemeBox.Unchecked += (sender, args) => CustomAppearanceGrid.IsEnabled = true;
		
		TileTypeBox.Items = Enum.GetNames(typeof(TileType)).Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"));
		if (this is ITileCreationPage)
			TileTypeBox.SelectedIndex = (int)((ITileCreationPage)this).CreationType;
		else
			throw new InvalidOperationException(
				"Tried to initialize TileTypeBox.SelectedIndex when it isn't part of ITileCreationPage");

		TransparencyModeBox.SelectionChanged += (sender, args) =>
		{
			var selection = (string?)TransparencyModeBox.SelectedItem ?? WindowTransparencyLevel.None.ToString();
			WarnUnsupportedOptionText.IsVisible = selection != WindowTransparencyLevel.None.ToString() &&
			                                      selection != WindowTransparencyLevel.Transparent.ToString();
			NewestWinOnlyText.IsVisible = selection == WindowTransparencyLevel.Mica.ToString();
			WarnOtherProgramsText.IsVisible = selection == WindowTransparencyLevel.Mica.ToString();
		};

		TileTypeBox.SelectionChanged += (sender, args) =>
		{
			var selection = (string?)TileTypeBox.SelectedItem ?? TileType.Tile.ToString();
			
			// Add spaces with regex to match selection.
			TilePathPanel.IsVisible = selection == Regex.Replace(TileType.DirectoryPortal.ToString(), "(\\B[A-Z])", " $1");
		};

		PathBox.Text = Configuration.GetDefaultTilesDirectory();
	}
	
	private void CreateTile()
	{
		// TODO: Save appearance.
		var id = Guid.NewGuid();
		Configuration.Tile.CreateTileConfig(App.Instance.Config, id, NameBox.Text, PathBox.Text);
		var window = new TileWindow();
		window.ID = id;
		window.Show();
	}
	
	private void CreateButton(object? sender, RoutedEventArgs e) => CreateTile();
}