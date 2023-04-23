using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Controls;

public partial class AppearanceComponent : UserControl
{
	public AppearanceComponent()
	{
		InitializeComponent();
		
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
	}
}