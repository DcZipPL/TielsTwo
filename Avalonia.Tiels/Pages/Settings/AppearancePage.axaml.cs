using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;

namespace Avalonia.Tiels.Pages.Settings;

public partial class AppearancePage : UserPage
{
	public AppearancePage()
	{
		InitializeComponent();

		// FIXME: Duplicate from TilePage
		TransparencyModeBox.Items = Enum.GetNames(typeof(WindowTransparencyLevel)).Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"));
		TransparencyModeBox.SelectedIndex = (int)App.Instance.Config.GlobalTransparencyLevel;

		ThemeBox.Items = Enum.GetNames(typeof(FluentThemeMode));
		ThemeBox.SelectedIndex = (int)App.Instance.Config.GlobalTheme;
		Util.SetSvgImage("/Assets/Icons/out/info.svg", ThemeInfoIcon);
		
		TransparencyModeBox.SelectionChanged += (sender, args) =>
		{
			var selection = (string?)TransparencyModeBox.SelectedItem ?? WindowTransparencyLevel.None.ToString();
			WarnUnsupportedOptionText.IsVisible = selection != WindowTransparencyLevel.None.ToString() &&
			                                      selection != WindowTransparencyLevel.Transparent.ToString();
			NewestWinOnlyText.IsVisible = selection == WindowTransparencyLevel.Mica.ToString();
			WarnOtherProgramsText.IsVisible = selection == WindowTransparencyLevel.Mica.ToString();
		};
	}
	
	private void ColorWheelSizeChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (ColorWheel != null)
		{
			ColorWheel.IsHSBVisible = ColorWheel.Bounds.Width > 890;
			ColorWheel.IsRGBVisible = ColorWheel.Bounds.Width > 890;
		}
	}
}