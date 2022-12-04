using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels.Pages.Create;

public partial class TilePage : UserPage
{
	private App _app = (App)Application.Current!;

	public TileType CreationType;
	
	public TilePage()
	{
		InitializeComponent();

		TransparencyModeBox.Items = Enum.GetNames(typeof(WindowTransparencyLevel)).Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"));
		TransparencyModeBox.SelectedIndex = (int)_app.Config.GlobalTransparencyLevel;
		
		UseGlobalThemeBox.Checked += (sender, args) => CustomAppearanceGrid.IsEnabled = false;
		UseGlobalThemeBox.Unchecked += (sender, args) => CustomAppearanceGrid.IsEnabled = true;
	}
}