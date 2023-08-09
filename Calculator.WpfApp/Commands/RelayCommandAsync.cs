using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.WpfApp.Commands;

internal class RelayCommandAsync : ICommand
{
	private readonly Func<object, Task> _execute;
	private readonly Func<object, bool> _canExecute;

	private long isExecuting;

	public RelayCommandAsync(Func<object, Task> execute, Func<object, bool>? canExecute = null)
	{
		_execute = execute;
		_canExecute = canExecute ?? (o => true);
	}

	public event EventHandler? CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}

	public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

	public bool CanExecute(object? parameter) => Interlocked.Read(ref isExecuting) == 0 && _canExecute(parameter);

	public async void Execute(object? parameter)
	{
		Interlocked.Exchange(ref isExecuting, 1);
		RaiseCanExecuteChanged();

		try
		{
			await _execute(parameter);
		}
		finally
		{
			Interlocked.Exchange(ref isExecuting, 0);
			RaiseCanExecuteChanged();
		}
	}
}