using Calculator.WpfApp.Models.Domains;
using Calculator.WpfApp.ViewModels;
using System.Windows;

namespace Calculator.WpfApp.Views;

/// <summary>
/// Interaction logic for HistoryView.xaml
/// </summary>
public partial class HistoryView : Window
{
	public HistoryViewModel HistoryViewModel { get; } = new HistoryViewModel();

	public HistoryView()
	{
		InitializeComponent();
		DataContext = HistoryViewModel;
	}
}