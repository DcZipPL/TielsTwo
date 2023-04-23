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
	}

	public override void ApplySettings()
	{
		
	}

	public override void RollbackSettings()
	{
		
	}
}