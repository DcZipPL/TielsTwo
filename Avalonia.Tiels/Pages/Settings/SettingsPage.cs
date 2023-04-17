using System;
using Avalonia.Controls;
using Avalonia.Tiels.Classes;

namespace Avalonia.Tiels.Pages.Settings;

public abstract class SettingsPage : UserPage
{
	protected (string, string?) status = (Icons.Check, App.I18n.GetString("NoChanges"));
	
	protected void ApplyIfChanged(bool getter, Action<bool> setter, CheckBox checkBox)
	{
		if (getter != checkBox.IsChecked && checkBox.IsChecked != null)
			setter(checkBox.IsChecked!.Value);
	}
	
	protected void ChangeSettingsStatus(string message, bool invalid)
	{
		status = (invalid ? Icons.X : Icons.Check, message);
	}

	public abstract void ApplySettings();
	public abstract void RollbackSettings();
}