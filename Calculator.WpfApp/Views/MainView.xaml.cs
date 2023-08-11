using System.Windows;
using System.Windows.Input;
using Calculator.WpfApp.ViewModels;
using MahApps.Metro.Controls;

namespace Calculator.WpfApp.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainView : MetroWindow
{
	public MainView()
	{
		InitializeComponent();
		DataContext = new MainViewModel();
	}

	private void Button_MouseMove(object sender, MouseEventArgs e)
	{
		ToolTipSettings.SetCurrentValue(System.Windows.Controls.ToolTip.PlacementProperty, System.Windows.Controls.Primitives.PlacementMode.Relative);
		ToolTipSettings.SetCurrentValue(System.Windows.Controls.ToolTip.HorizontalOffsetProperty, e.GetPosition((IInputElement)sender).X + 16);
		ToolTipSettings.SetCurrentValue(System.Windows.Controls.ToolTip.VerticalOffsetProperty, e.GetPosition((IInputElement)sender).Y + 16);
	}
}