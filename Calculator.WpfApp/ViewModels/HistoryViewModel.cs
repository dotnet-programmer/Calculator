using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Calculator.WpfApp.Commands;
using Calculator.WpfApp.Models.Domains;
using Calculator.WpfApp.Repositories;

namespace Calculator.WpfApp.ViewModels;

public class HistoryViewModel : BaseViewModel
{
	private int _selectedIndex;

	public HistoryViewModel()
	{
		SetCommands();
		RefreshHistory().Wait();
	}

	#region Property binding

	private Result? _selectedResult;
	public Result? SelectedResult
	{
		get => _selectedResult;
		set { _selectedResult = value; OnPropertyChanged(); }
	}

	private ObservableCollection<Result> _results;
	public ObservableCollection<Result> Results
	{
		get => _results;
		set { _results = value; OnPropertyChanged(); }
	}

	#endregion Property binding

	public ICommand DeleteValueCommand { get; private set; }
	public ICommand MoveSelectionCommand { get; private set; }
	public ICommand CloseWindowCommand { get; private set; }

	private void SetCommands()
	{
		DeleteValueCommand = new RelayCommandAsync(DeleteValueAsync);
		MoveSelectionCommand = new RelayCommand(MoveSelection);
		CloseWindowCommand = new RelayCommand(CloseWindow);
	}

	private async Task DeleteValueAsync(object commandParameter)
	{
		if (SelectedResult is not null)
		{
			_selectedIndex = Results.IndexOf(SelectedResult);
			await ResultRepository.DeleteResultAsync(SelectedResult.ResultId);
			await RefreshHistory();
		}
	}

	private void MoveSelection(object commandParameter)
	{
		if (commandParameter is Key key)
		{
			if (key == Key.Up && _selectedIndex > 0)
			{
				_selectedIndex--;
				SelectedResult = Results[_selectedIndex];
			}

			if (key == Key.Down && _selectedIndex < Results.Count - 1)
			{
				_selectedIndex++;
				SelectedResult = Results[_selectedIndex];
			}
		}
	}

	private void CloseWindow(object commandParameter)
	{
		SelectedResult = null;
		(commandParameter as Window)?.Close();
	}

	private async Task RefreshHistory()
	{
		Results = new(await ResultRepository.GetResultsAsync());
		if (Results.Count > 0)
		{
			if (_selectedIndex > Results.Count - 1)
			{
				_selectedIndex--;
			}
			SelectedResult = Results[_selectedIndex];
		}
	}
}