using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels.Pages.Settings;

public partial class ExperimentalPage : SettingsPage
{
	public ExperimentalPage()
	{
		InitializeComponent();
	}

	public override void ApplySettings()
	{
		throw new System.NotImplementedException();
	}

	public override void RollbackSettings()
	{
		throw new System.NotImplementedException();
	}

	private void ExEnableOsBypassCheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e) => ((CheckBox)sender!).IsChecked = false;
	private void ExEnableDragAndDropCheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e) => ((CheckBox)sender!).IsChecked = false;
	private void ExEnableSimpleModeCheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e) => ((CheckBox)sender!).IsChecked = false;
	private void ExEnableNotesCategoryCheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e) => ((CheckBox)sender!).IsChecked = false;
	private void ExEnableFloatingImagesCategoryCheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e) => ((CheckBox)sender!).IsChecked = false;
}