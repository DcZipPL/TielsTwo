using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Avalonia.Tiels
{
	public partial class App : Application
	{
		public Configuration Config { get; set; }
		
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IControlledApplicationLifetime desktop)
			{
				ExecuteApplication(desktop);
			}

			base.OnFrameworkInitializationCompleted();
		}

		private void ExecuteApplication(IControlledApplicationLifetime desktop)
		{
			if (Configuration.IsFirstStartup())
			{
				Config = Configuration.Init(desktop);
			}
			else
			{
				// Load tiles
				Config = Configuration.Load(desktop);
			}
		}
	}
}