using System;
using Avalonia.Themes.Fluent;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Pages.Settings;

public partial class AppearancePage : SettingsPage
{
	public AppearancePage()
	{
		InitializeComponent();
		
		ApplicationThemeBox.ItemsSource = Enum.GetNames(typeof(ThemeMode));
		ApplicationThemeBox.SelectedIndex = (int)App.Instance.Config.GlobalTheme;
	}

	public override void ApplySettings()
	{
		if (App.Instance.Config.GlobalTheme != (ThemeMode)ApplicationThemeBox.SelectedIndex)
		{
			App.Instance.Config.GlobalTheme = (ThemeMode)ApplicationThemeBox.SelectedIndex;
			foreach (var styles in App.Instance.Styles)
			{
				/*if (styles is FluentTheme fluentTheme) TODO: Fix themes
				{
					styles. = (ThemeMode)ApplicationThemeBox.SelectedIndex;
					break;
				}*/
			}
		}

		ChangeSettingsStatus(App.I18n.GetString("ChangesAccepted"), false);
	}

	public override void RollbackSettings()
	{
		
	}
}