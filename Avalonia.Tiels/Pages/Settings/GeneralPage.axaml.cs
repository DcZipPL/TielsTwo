using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Pages.Settings;

public partial class GeneralPage : UserPage
{
	private (string, string?) status = (Icons.Check, App.I18n.GetString("NoChanges"));

	internal SettingsWindow Root;

	public GeneralPage()
	{
		InitializeComponent();
		LoadSettingsValues();
	}

	private void ApplySettings()
	{
		// Apply snapping settings
		if (App.Instance.Config.Snapping.ToString() != SnappingBox.Text)
			if (float.TryParse(SnappingBox.Text, out float snapping))
				App.Instance.Config.Snapping = snapping;
			else
				ChangeSettingsStatus(App.I18n.GetString("InvalidInput")!, true);
		
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
		ApplyIfChanged(App.Instance.Config.Autostart, b => App.Instance.Config.Autostart = b, AutostartCheckBox);
		ApplyIfChanged(App.Instance.Config.AutostartHideSettings, b => App.Instance.Config.AutostartHideSettings = b, HideWindowCheckBox);
		ApplyIfChanged(App.Instance.Config.SpecialEffects, b => App.Instance.Config.SpecialEffects = b, EffectsCheckBox);
		ApplyIfChanged(App.Instance.Config.Experimental, b => App.Instance.Config.Experimental = b, ExperimentalCheckBox);
		ApplyIfChanged(App.Instance.Config.HideTileButtons, b => App.Instance.Config.HideTileButtons = b, HideTileButtonsCheckBox);

		// Save draft data before reinitialize
		var draftTilesPath = TilesDirectoryBox.Text;

		// Reinitialize everything if language is changed
		Root.LoadWindow();
		InitializeComponent();
		LoadSettingsValues();

		// Load draft data
		TilesDirectoryBox.Text = draftTilesPath;

		// Show status
		StatusIcon.Text = status.Item1;
		StatusText.Text = status.Item2;
	}

	private void ChangeSettingsStatus(string message, bool invalid)
	{
		status = (invalid ? Icons.X : Icons.Check, message);
	}

	private void RollbackSettings()
	{
	}

	private void LoadSettingsValues()
	{
		SnappingBox.Text = App.Instance.Config.Snapping.ToString();
		
		TilesDirectoryBox.Text = App.Instance.Config.TilesPath;
		TilesDirectoryBox.Watermark = Configuration.GetDefaultTilesDirectory();

		LanguageBox.Items = Util.ImplementedCultures();
		LanguageBox.SelectedItem = ((IEnumerable<CultureInfo>)LanguageBox.Items).ToList()
			.FirstOrDefault(item => Equals(item, CultureInfo.CurrentUICulture),
				((IEnumerable<CultureInfo>)LanguageBox.Items).ToList()[0]);

		AutostartCheckBox.IsChecked = App.Instance.Config.Autostart;
		HideWindowCheckBox.IsChecked = App.Instance.Config.AutostartHideSettings;
		EffectsCheckBox.IsChecked = App.Instance.Config.SpecialEffects;
		ExperimentalCheckBox.IsChecked = App.Instance.Config.Experimental;
		HideTileButtonsCheckBox.IsChecked = App.Instance.Config.HideTileButtons;
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

	private void ApplyIfChanged(bool getter, Action<bool> setter, CheckBox checkBox)
	{
		if (getter != checkBox.IsChecked && checkBox.IsChecked != null)
			setter(checkBox.IsChecked!.Value);
	}

	private void LanguageChanged(object? sender, SelectionChangedEventArgs e)
	{
		try
		{
			var ci = (CultureInfo)LanguageBox.SelectedItem;
			CultureInfo.CurrentCulture = ci;
			CultureInfo.CurrentUICulture = ci;
		}
		catch (Exception ex)
		{
			throw ErrorHandler.ShowErrorWindow(ex, 0x0003);
		}
	}

	#region Boilerplate
	private void ApplyButtonClicked(object? sender, RoutedEventArgs e) => ApplySettings();
	private void DefaultButtonClicked(object? sender, RoutedEventArgs e) => RollbackSettings();
	#endregion
}