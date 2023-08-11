using Calculator.WpfApp.ViewModels;
using MahApps.Metro.Controls;

namespace Calculator.WpfApp.Views;

/// <summary>
/// Interaction logic for SettingsView.xaml
/// </summary>
public partial class SettingsView : MetroWindow
{
	public SettingsView()
	{
		InitializeComponent();
		DataContext = new SettingsViewModel();
	}
}