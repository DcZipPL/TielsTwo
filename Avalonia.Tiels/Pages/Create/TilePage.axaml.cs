using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;

namespace Avalonia.Tiels.Pages.Create;

public partial class TilePage : UserPage
{
	private App _app = (App)Application.Current!;

	public TilePage()
	{
		InitializeComponent();

		TransparencyModeBox.Items = Enum.GetNames(typeof(WindowTransparencyLevel)).Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"));
		TransparencyModeBox.SelectedIndex = (int)_app.Config.GlobalTransparencyLevel;

		ThemeBox.Items = Enum.GetNames(typeof(FluentThemeMode));
		ThemeBox.SelectedIndex = (int)_app.Config.GlobalTheme;
		
		UseGlobalThemeBox.Checked += (sender, args) => CustomAppearanceGrid.IsEnabled = false;
		UseGlobalThemeBox.Unchecked += (sender, args) => CustomAppearanceGrid.IsEnabled = true;
		
		TileTypeBox.Items = Enum.GetNames(typeof(TileType)).Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"));
		if (this is ITileCreationPage)
			TileTypeBox.SelectedIndex = (int)((ITileCreationPage)this).CreationType;
		else
			throw new InvalidOperationException(
				"Tried to initialize TileTypeBox.SelectedIndex when it isn't party of ITileCreationPage");

		TransparencyModeBox.SelectionChanged += (sender, args) =>
		{
			var selection = (string?)TransparencyModeBox.SelectedItem ?? WindowTransparencyLevel.None.ToString();
			WarnUnsupportedOptionText.IsVisible = selection != WindowTransparencyLevel.None.ToString() &&
			                                      selection != WindowTransparencyLevel.Transparent.ToString();
			NewestWinOnlyText.IsVisible = selection == WindowTransparencyLevel.Mica.ToString();
		};
	}
}