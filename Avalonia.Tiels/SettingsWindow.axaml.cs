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
				SB_CT.WithPage(TileCreatePage.RootOf(this)),
				SB_CDP.WithPage(DirectoryPortalPage.SetPageAsDirectoryPortal().RootOf(this)),
				SB_CFI.WithPage(FloatingImagePage.RootOf(this)),
				SB_CN.WithPage(NotesPage.RootOf(this)),
				SB_SG.WithPage(GeneralSettingsPage.RootOf(this)),
				SB_SA.WithPage(AppearanceSettingsPage.RootOf(this)),
				SB_ST.WithPage(ThumbnailSettingsPage.RootOf(this)),
				SB_SU.WithPage(UpdatesSettingsPage.RootOf(this)),
				SB_SE.WithPage(ExperimentalSettingsPage.RootOf(this)),
				SB_MT.WithPage(SnappingSettingsPage.RootOf(this)),
				SB_AB.WithPage(AboutSettingsPage.RootOf(this)),
				SB_MFI.WithPage(null),
				SB_MN.WithPage(null)
			};

			SB_SE.IsVisible = App.Instance.Config.Experimental;
			SB_ST.IsVisible = App.Instance.Config.Experimental;
			SB_SU.IsVisible = App.Instance.Config.Experimental;
			
			SB_SU.IsEnabled = App.Instance.Config.UpdatesSettingsEnabled;
			SB_ST.IsEnabled = App.Instance.Config.ThumbnailsSettingsEnabled;

			// Load default page
			LoadDefaultPage();

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

								// Reset contracts
								if (sidebarButton.Page is SettingsPage settingsPage)
								{
									ApplyButtonContract = (_, _) => { settingsPage.ApplySettings(); };
									DefaultButtonContract = (_, _) => { settingsPage.RollbackSettings(); };
								} else if (sidebarButton.Page is TilePage tileCreatePage)
								{
									CreateButtonContract = (_, _) => { tileCreatePage.CreateTile(); };
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

		private void LoadDefaultPage()
		{
			IPage.ChangeVisibility(GeneralSettingsPage, true);
			StatusPanel.IsVisible = true;
			SettingsControl.IsVisible = true;
			ApplyButtonContract = (_, _) => { GeneralSettingsPage.ApplySettings(); };
			DefaultButtonContract = (_, _) => { GeneralSettingsPage.RollbackSettings(); };
		}

		internal void LoadWindow()
		{
			InitializeComponent();

			LoadSidebar();
		}

		#region Contracts

		private System.EventHandler<Interactivity.RoutedEventArgs> _applyButtonContract;
		private System.EventHandler<Interactivity.RoutedEventArgs> _defaultButtonContract;
		private System.EventHandler<Interactivity.RoutedEventArgs> _createButtonContract;

		private System.EventHandler<Interactivity.RoutedEventArgs> ApplyButtonContract
		{
			get => _applyButtonContract;
			set {
				if (_applyButtonContract != null)
					ApplyButton.Click -= _applyButtonContract;
				_applyButtonContract = value;
				ApplyButton.Click += _applyButtonContract;
			}
		}
		private System.EventHandler<Interactivity.RoutedEventArgs> DefaultButtonContract
		{
			get => _defaultButtonContract;
			set {
				if (_defaultButtonContract != null)
					DefaultButton.Click -= _defaultButtonContract;
				_defaultButtonContract = value;
				DefaultButton.Click += _defaultButtonContract;
			}
		}
		private System.EventHandler<Interactivity.RoutedEventArgs> CreateButtonContract
		{
			get => _createButtonContract;
			set {
				if (_createButtonContract != null)
					CreateControl.Click -= _createButtonContract;
				_createButtonContract = value;
				CreateControl.Click += _createButtonContract;
			}
		}

		#endregion
	}
}