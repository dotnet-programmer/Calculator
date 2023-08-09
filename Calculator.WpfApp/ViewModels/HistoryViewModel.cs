using Calculator.WpfApp.Commands;
using Calculator.WpfApp.Models;
using Calculator.WpfApp.Models.Domains;
using Calculator.WpfApp.Repositories;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.WpfApp.ViewModels;

public class HistoryViewModel : BaseViewModel
{
	private int _selectedIndex;

	public HistoryViewModel()
	{
		SetCommands();
		RefreshHistory();
	}

	private Result _selectedResult;
	public Result SelectedResult
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

	public Result HistoryResult { get; private set; }

	public ICommand GridDoubleClickCommand { get; private set; }
	public ICommand DeleteValueAsyncCommand { get; private set; }
	public ICommand MoveSelectionCommand { get; private set; }

	private void SetCommands()
	{
		GridDoubleClickCommand = new RelayCommand(GridDoubleClick);
		DeleteValueAsyncCommand = new RelayCommandAsync(DeleteValueAsync);
		MoveSelectionCommand = new RelayCommand(MoveSelection);
	}

	private async Task RefreshHistory()
	{
		Results = new ObservableCollection<Result>(await ResultRepository.GetResultsAsync());
		if (Results.Count > 0)
		{
			if (_selectedIndex > Results.Count - 1)
			{
				_selectedIndex--;
			}
			SelectedResult = Results[_selectedIndex];
		}
	}

	private void GridDoubleClick(object obj)
	{
		HistoryViewParams historyViewParams = obj as HistoryViewParams;
		HistoryResult = historyViewParams.Result;
		historyViewParams.Window.Close();
	}

	private async Task DeleteValueAsync(object obj)
	{
		if (SelectedResult is not null)
		{
			_selectedIndex = Results.IndexOf(SelectedResult);
			await ResultRepository.DeleteResultAsync(SelectedResult.ResultId);
			await RefreshHistory();
		}
	}

	private void MoveSelection(object obj)
	{
		if (obj is Key key)
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
}