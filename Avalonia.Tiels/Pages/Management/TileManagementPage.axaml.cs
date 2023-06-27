using System;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Tiels.Classes;
using Avalonia.Tiels.Controls;

namespace Avalonia.Tiels.Pages.Settings;

public partial class SnappingPage : SettingsPage
{
	public SnappingPage()
	{
		InitializeComponent();
		LoadSettingsValues();
		LoadTileList();
	}

	private void LoadTileList()
	{
		try
		{
			foreach (var tile in App.Instance.Config.Tiles)
			{
				TileManagementEntries.Children.Add(new TileManageListEntry
				{
					ID = Guid.Parse(tile.Value.Id),
					Type = TileType.Tile, // TODO: Get type of tile
					Text = tile.Value.Name
				});
			}
		} catch (Exception e)
		{
			// TODO: Show in app error that tile list could not be loaded.
			LoggingHandler.Warn(nameof(LoadTileList), e.ToString());
		}
	}

	public override void RollbackSettings()
	{
	}
	
	public override void ApplySettings()
	{
		// Apply snapping settings
		if (App.Instance.Config.Snapping.ToString() != SnappingBox.Text)
			if (float.TryParse(SnappingBox.Text, out float snapping))
				App.Instance.Config.Snapping = snapping;
			else
				ChangeSettingsStatus(App.I18n.GetString("InvalidInput")!, true);
		
		// Apply handle height settings
		if (App.Instance.Config.HandleHeight.ToString() != HandleSizeBox.Text)
			if (float.TryParse(HandleSizeBox.Text, out float snapping))
				App.Instance.Config.HandleHeight = snapping;
			else
				ChangeSettingsStatus(App.I18n.GetString("InvalidInput")!, true);
		
		ApplyIfChanged(App.Instance.Config.HideTileButtons, b => App.Instance.Config.HideTileButtons = b, HideTileButtonsCheckBox);
		
		LoadSettingsValues();
		
		// Show status
		Root.StatusIcon.Text = status.Item1;
		Root.StatusText.Text = status.Item2;
	}

	private void LoadSettingsValues()
	{
		SnappingBox.Text = App.Instance.Config.Snapping.ToString();
		HandleSizeBox.Text = App.Instance.Config.HandleHeight.ToString();
		
		HideTileButtonsCheckBox.IsChecked = App.Instance.Config.HideTileButtons;
	}
	
	#region Boilerplate
	private void ApplyButtonClicked(object? sender, RoutedEventArgs e) => ApplySettings();
	private void DefaultButtonClicked(object? sender, RoutedEventArgs e) => RollbackSettings();
	#endregion
}