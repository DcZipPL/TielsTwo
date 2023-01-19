namespace Avalonia.Tiels.Pages;

public interface IPage
{
	public static void ChangeVisibility(UserPage? userPage, bool isVisible)
	{
		if (userPage == null) return;
		userPage.IsVisible = isVisible;
	}
}