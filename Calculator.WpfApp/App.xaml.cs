using System.Windows;
using Calculator.WpfApp.Properties;
using ControlzEx.Theming;

namespace Calculator.WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		if (Settings.Default.IsWindowsThemeUse)
		{
			ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
			ThemeManager.Current.SyncTheme();
		}
		else
		{
			ThemeManager.Current.ChangeTheme(this, $"{Settings.Default.BaseScheme}.{Settings.Default.ColorScheme}");
		}
	}
}