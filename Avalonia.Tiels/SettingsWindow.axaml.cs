using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Tiels.Controls;
using Avalonia.Tiels.Pages;

namespace Avalonia.Tiels
{
	public partial class SettingsWindow : Window
	{
		private List<SidebarButton> _sidebarButtons;

		public SettingsWindow()
		{
			LoadWindow();
		}

		private void LoadSidebar()
		{
			// TODO: Less hardcoding pls
			_sidebarButtons = new List<SidebarButton>
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
			
			foreach (var sidebarButton in _sidebarButtons)
			{
				sidebarButton.Top.Click += (rawSender, args) =>
				{
					// Change page if checked
					var sender = (ToggleButton)rawSender!;
					if (sender.IsChecked.HasValue && sender.IsChecked.Value)
					{
						foreach (var sidebarButton in _sidebarButtons)
						{
							if (sidebarButton.Top.Equals(sender))
							{
								IPage.ChangeVisibility(sidebarButton.Page, true);
							}
							else
							{
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