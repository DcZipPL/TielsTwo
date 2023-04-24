using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;

namespace Avalonia.Tiels.Pages.Settings;

public partial class AppearancePage : SettingsPage
{
	public AppearancePage()
	{
		InitializeComponent();
		
		ApplicationThemeBox.Items = Enum.GetNames(typeof(FluentThemeMode));
		ApplicationThemeBox.SelectedIndex = (int)App.Instance.Config.GlobalTheme;
	}

	public override void ApplySettings()
	{
		if (App.Instance.Config.GlobalTheme != (FluentThemeMode)ApplicationThemeBox.SelectedIndex)
		{
			App.Instance.Config.GlobalTheme = (FluentThemeMode)ApplicationThemeBox.SelectedIndex;
			foreach (var styles in App.Instance.Styles)
			{
				if (styles is FluentTheme fluentTheme)
				{
					fluentTheme.Mode = (FluentThemeMode)ApplicationThemeBox.SelectedIndex;
					break;
				}
			}
		}

		ChangeSettingsStatus(App.I18n.GetString("ChangesAccepted"), false);
	}

	public override void RollbackSettings()
	{
		
	}
}