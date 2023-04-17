using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Tiels.Controls;
using Avalonia.Tiels.Pages;
using Avalonia.Tiels.Pages.Create;
using Avalonia.Tiels.Pages.Settings;

namespace Avalonia.Tiels
{
	public partial class SettingsWindow : Window
	{
		private System.EventHandler<Avalonia.Interactivity.RoutedEventArgs>? applyButtonContract;
		private System.EventHandler<Avalonia.Interactivity.RoutedEventArgs>? defaultButtonContract;
		private System.EventHandler<Avalonia.Interactivity.RoutedEventArgs>? createButtonContract;

		public SettingsWindow()
		{
			LoadWindow();
		}

		private void LoadSidebar()
		{
			List<SidebarButton> sidebarButtons = new List<SidebarButton>
			{
				SB_CT.WithPage(TileCreatePage),
				SB_CDP.WithPage(DirectoryPortalPage.SetPageAsDirectoryPortal()),
				SB_CFI.WithPage(FloatingImagePage),
				SB_CN.WithPage(NotesPage),
				SB_SG.WithPage(GeneralSettingsPage),
				SB_SA.WithPage(AppearanceSettingsPage),
				SB_ST.WithPage(ThumbnailSettingsPage),
				SB_SU.WithPage(UpdatesSettingsPage),
				SB_SE.WithPage(ExperimentalSettingsPage),
				SB_MT.WithPage(SnappingSettingsPage),
				SB_MFI.WithPage(null),
				SB_MN.WithPage(null)
			};
			
			SB_SE.IsVisible = App.Instance.Config.Experimental;
			SB_ST.IsVisible = App.Instance.Config.Experimental;
			SB_ST.IsEnabled = App.Instance.Config.ThumbnailsSettingsEnabled;
			
			// Load default page
			applyButtonContract = (_, _) => { GeneralSettingsPage.ApplySettings(); };
			defaultButtonContract = (_, _) => { GeneralSettingsPage.RollbackSettings(); Serilog.Log.Information(GeneralSettingsPage.GetType().Name); };

			IPage.ChangeVisibility(GeneralSettingsPage, true);
			StatusPanel.IsVisible = true;
			SettingsControl.IsVisible = true;
			ApplyButton.Click += applyButtonContract;
			DefaultButton.Click += defaultButtonContract;

			// Add click events for sidebar buttons
			foreach (var sidebarButton in sidebarButtons)
			{
				sidebarButton.Top.Click += (rawSender, args) =>
				{
					// Change page if checked
					var sender = (ToggleButton)rawSender!;
					if (sender.IsChecked.HasValue && sender.IsChecked.Value)
					{
						foreach (var sidebarButton in sidebarButtons)
						{
							if (sidebarButton.Top.Equals(sender))
							{
								// Enable page
								IPage.ChangeVisibility(sidebarButton.Page, true);
								
								StatusPanel.IsVisible = sidebarButton.Page is SettingsPage;
								SettingsControl.IsVisible = sidebarButton.Page is SettingsPage;
								CreateControl.IsVisible = sidebarButton.Page is TilePage;
								
								if (sidebarButton.Page is SettingsPage settingsPage)
								{
									// Clear contracts
									ApplyButton.Click -= applyButtonContract;
									DefaultButton.Click -= defaultButtonContract;

									// Save new contracts
									applyButtonContract = (_, _) => { settingsPage.ApplySettings(); };
									defaultButtonContract = (_, _) => { settingsPage.RollbackSettings(); Serilog.Log.Information(settingsPage.GetType().Name); };

									// Apply new to buttons
									ApplyButton.Click += applyButtonContract;
									DefaultButton.Click += defaultButtonContract;
								} else if (sidebarButton.Page is TilePage tileCreatePage) {
									// Clear contracts
									CreateControl.Click -= createButtonContract;

									// Save new contracts
									createButtonContract = (_, _) => { tileCreatePage.CreateTile(); };

									// Apply new to buttons
									CreateControl.Click += createButtonContract;
								
								}
							}
							else
							{
								// Disable page
								sidebarButton.Top.IsChecked = false;
								IPage.ChangeVisibility(sidebarButton.Page, false);
							}
						}
					}
					else
					{
						// Disable unchecking
						sender.IsChecked = true;
					}
				};
			}
		}

		internal void LoadWindow()
		{
			InitializeComponent();
			GeneralSettingsPage.Root = this;
			
			LoadSidebar();
		}
	}
}