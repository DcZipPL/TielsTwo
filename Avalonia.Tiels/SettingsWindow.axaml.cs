using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Tiels.Controls;
using Avalonia.Tiels.Pages;
using Avalonia.Tiels.Pages.Create;

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
				SB_CDP.WithPage(DirectoryPortalPage),
				SB_CFI.WithPage(FloatingImagePage),
				SB_CN.WithPage(NotesPage),
				SB_SG.WithPage(GeneralSettingsPage),
				SB_SA.WithPage(AppearanceSettingsPage),
				SB_SU.WithPage(UpdatesSettingsPage),
				SB_MT.WithPage(null),
				SB_MFI.WithPage(null),
				SB_MN.WithPage(null)
			};
			
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