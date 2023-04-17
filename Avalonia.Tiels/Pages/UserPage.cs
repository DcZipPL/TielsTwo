using Avalonia.Controls;

namespace Avalonia.Tiels.Pages;

public abstract class UserPage : UserControl, IPage
{
	public UserPage RootOf(SettingsWindow parent)
	{
		Root = parent;
		return this;
	}

	internal SettingsWindow Root;
}