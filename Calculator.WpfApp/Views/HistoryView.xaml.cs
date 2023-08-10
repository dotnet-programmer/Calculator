using Calculator.WpfApp.ViewModels;
using MahApps.Metro.Controls;

namespace Calculator.WpfApp.Views;

/// <summary>
/// Interaction logic for HistoryView.xaml
/// </summary>
public partial class HistoryView : MetroWindow
{
	public HistoryViewModel HistoryViewModel { get; } = new HistoryViewModel();

	public HistoryView()
	{
		InitializeComponent();
		DataContext = HistoryViewModel;
	}
}