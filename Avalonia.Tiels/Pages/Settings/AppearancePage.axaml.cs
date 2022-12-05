using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Avalonia.Tiels.Pages.Settings;

public partial class AppearancePage : UserPage
{
	public AppearancePage()
	{
		InitializeComponent();
	}
	
	private void ColorWheelSizeChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (ColorWheel != null)
		{
			ColorWheel.IsHSBVisible = ColorWheel.Bounds.Width > 890;
			ColorWheel.IsRGBVisible = ColorWheel.Bounds.Width > 890;
		}
	}
}