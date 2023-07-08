using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Controls;

public partial class AppearanceComponent : UserControl
{
	public AppearanceComponent()
	{
		InitializeComponent();
		
		TransparencyModeBox.ItemsSource = Enum.GetNames(typeof(TransparencyLevel)).Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"));
		TransparencyModeBox.SelectedIndex = (int)App.Instance.Config.GlobalTransparencyLevel;

		ThemeBox.ItemsSource = Enum.GetNames(typeof(ThemeMode));
		ThemeBox.SelectedIndex = (int)App.Instance.Config.GlobalTheme;

		UseGlobalThemeBox.Checked += (sender, args) => CustomAppearanceGrid.IsEnabled = false;
		UseGlobalThemeBox.Unchecked += (sender, args) => CustomAppearanceGrid.IsEnabled = true;

		ColorBtn.Color = App.Instance.Config.GlobalTheme == ThemeMode.Dark ? Palette.TILE_DARK_COLOR : Palette.TILE_LIGHT_COLOR;
		ThemeBox.SelectionChanged += (_, _) =>
		{
			if (ColorBtn.Color == Palette.TILE_DARK_COLOR || ColorBtn.Color == Palette.TILE_LIGHT_COLOR)
				ColorBtn.Color = (ThemeMode)ThemeBox.SelectedIndex == ThemeMode.Dark ? Palette.TILE_DARK_COLOR: Palette.TILE_LIGHT_COLOR;
		};
		
		TransparencyModeBox.SelectionChanged += (_, _) =>
		{
			var selection = (string?)TransparencyModeBox.SelectedItem ?? TransparencyLevel.None.ToString();
			WarnUnsupportedOptionText.IsVisible = selection != TransparencyLevel.None.ToString() &&
			                                      selection != TransparencyLevel.Transparent.ToString();
			NewestWinOnlyText.IsVisible = selection == TransparencyLevel.Mica.ToString();
			WarnOtherProgramsText.IsVisible = selection == TransparencyLevel.Mica.ToString();
		};
	}
}