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
			IPage.ChangeVisibility(GeneralSettingsPage, true);
			StatusPanel.IsVisible = true;
			SettingsControl.IsVisible = true;
			ApplyButton.Click += (_, _) => GeneralSettingsPage.ApplySettings();
			DefaultButton.Click += (_, _) => GeneralSettingsPage.RollbackSettings();

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
								
								if (sidebarButton.Page is not SettingsPage settingsPage) continue;
								ApplyButton.Click += (_, _) => settingsPage.ApplySettings(); // TODO: Check for duplicated events
								DefaultButton.Click += (_, _) => settingsPage.RollbackSettings(); // TODO: Make and save delegate that will be removed
							}
							else
							{
								// Disable page
								sidebarButton.Top.IsChecked = false;
								IPage.ChangeVisibility(sidebarButton.Page, false);
								
								if (sidebarButton.Page is not SettingsPage settingsPage) continue;
								ApplyButton.Click -= (_, _) => settingsPage.ApplySettings();
								DefaultButton.Click -= (_, _) => settingsPage.RollbackSettings();
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