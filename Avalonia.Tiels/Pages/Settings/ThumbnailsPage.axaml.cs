namespace Avalonia.Tiels.Pages.Settings;

public partial class ThumbnailsPage : SettingsPage
{
	public ThumbnailsPage()
	{
		InitializeComponent();
		// 3 normal, 4 normal again, 5,6 content view, 8 gray, 9 vista
		//ThumbnailCsi.GetShellIcon(3);
	}

	public override void ApplySettings()
	{
		throw new System.NotImplementedException();
	}

	public override void RollbackSettings()
	{
		throw new System.NotImplementedException();
	}
}