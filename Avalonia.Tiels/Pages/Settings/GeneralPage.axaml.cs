using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Pages.Settings;

public partial class GeneralPage : SettingsPage
{
	public GeneralPage()
	{
		InitializeComponent();
		LoadSettingsValues();
	}

	public override void ApplySettings()
	{
		// Check if tiles path didn't changed. If changed apply new location if valid to config and give status.
		var newPath = string.IsNullOrEmpty(TilesDirectoryBox.Text)
			? Configuration.GetDefaultTilesDirectory()
			: TilesDirectoryBox.Text;

		if (App.Instance.Config.TilesPath != newPath)
			if (Directory.Exists(newPath))
			{
				App.Instance.Config.TilesPath = TilesDirectoryBox.Text;
				ChangeSettingsStatus(App.I18n.GetString("ChangesAccepted")!, false);
			}
			else
			{
				ChangeSettingsStatus(App.I18n.GetString("PathDontExist")!, true);
			}
		else
			ChangeSettingsStatus(App.I18n.GetString("NoChanges")!, false);

		// Apply selections to config
		ApplyIfChanged(App.Instance.Config.AutoStart, b => App.Instance.Config.AutoStart = b, AutostartCheckBox);
		ApplyIfChanged(App.Instance.Config.AutoStartHideSettings, b => App.Instance.Config.AutoStartHideSettings = b, HideWindowCheckBox);
		ApplyIfChanged(App.Instance.Config.Experimental, b => App.Instance.Config.Experimental = b, ExperimentalCheckBox);
		
		// Apply language change
		App.Instance.Config.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

		// Save draft data before reinitialize
		var draftTilesPath = TilesDirectoryBox.Text;

		// Reinitialize everything if language is changed
		Root.LoadWindow();
		InitializeComponent();
		LoadSettingsValues();

		// Load draft data
		TilesDirectoryBox.Text = draftTilesPath;

		// Show status
		Root.StatusIcon.Text = Status.Item1;
		Root.StatusText.Text = Status.Item2;
	}

	public override void RollbackSettings()
	{
	}

	private void LoadSettingsValues()
	{
		TilesDirectoryBox.Text = App.Instance.Config.TilesPath;
		TilesDirectoryBox.Watermark = Configuration.GetDefaultTilesDirectory();

		LanguageBox.ItemsSource = Util.ImplementedCultures();
		LanguageBox.SelectedItem = ((IEnumerable<CultureInfo>)LanguageBox.ItemsSource).ToList()
			.FirstOrDefault(item => Equals(item, CultureInfo.CurrentUICulture),
				((IEnumerable<CultureInfo>)LanguageBox.ItemsSource).ToList()[0]);

		AutostartCheckBox.IsChecked = App.Instance.Config.AutoStart;
		HideWindowCheckBox.IsChecked = App.Instance.Config.AutoStartHideSettings;
		ExperimentalCheckBox.IsChecked = App.Instance.Config.Experimental;
	}

	private void TilePathTextBoxChanged(object? sender, KeyEventArgs e)
	{
		if (!Directory.Exists(TilesDirectoryBox.Text))
		{
			TilesDirectoryBox.BorderBrush = new SolidColorBrush(0xFFFF0000);
			TilesDirectoryBox.Foreground = new SolidColorBrush(0xFFFF0000);
		}
		else
		{
			// TODO: Use resources and not hardcoded values
			TilesDirectoryBox.BorderBrush = new SolidColorBrush(0xFF2A2A2A);
			TilesDirectoryBox.Foreground = new SolidColorBrush(0xFF000000);
		}
	}

	private void LanguageChanged(object? sender, SelectionChangedEventArgs e)
	{
		var selected = LanguageBox.SelectedItem;
		if (selected == null) throw LoggingHandler.Error(new NullReferenceException("No language selected"), nameof(GeneralPage));
		var ci = (CultureInfo)selected;
		CultureInfo.CurrentCulture = ci;
		CultureInfo.CurrentUICulture = ci;
	}
}