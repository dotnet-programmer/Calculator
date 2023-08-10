using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Calculator.WpfApp.Commands;
using Calculator.WpfApp.Models.Calculation;
using Calculator.WpfApp.Models.Domains;
using Calculator.WpfApp.Repositories;
using Calculator.WpfApp.Views;

namespace Calculator.WpfApp.ViewModels;

internal class MainViewModel : BaseViewModel
{
	private const string EXPRESSION_START_CHAR = "_";
	private const string RESULT_START_CHAR = "0";
	private const char SQUARE_ROOT_CHAR = '√';
	private const char US_DECIMAL_SEPARATOR = '.';
	private const char OPEN_BRACKET = '(';
	private const char CLOSE_BRACKET = ')';

	private readonly char _separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
	private readonly List<char> _availableOperations = new() { '+', '-', '*', '/', '^' };
	private readonly DataTableMethod _dataTableMethod = new();
	private readonly ExpressionParserMethod _expressionParserMethod = new();
	private readonly InfixToPostfixMethod _infixToPostfixMethod = new();

	private ICalculate _calculationMethod;

	private int _openBracket;
	private int _closeBracket;

	private bool _isOperation;
	private bool _isMinusNumber;
	private bool _isResultDisplayed;
	private bool _canDeleteLastChar;
	private bool _isSquareRootInNumber;
	private bool _isDecimalPointInNumber;

	public MainViewModel()
	{
		SetCommands();
		SetStartState();
		_calculationMethod = _infixToPostfixMethod;
	}

	private bool IsTheResultDisplayedAndNotZero => _isResultDisplayed && ScreenResult is not RESULT_START_CHAR;
	private bool IsSeparator => ScreenExpression.Last() == _separator;
	private bool IsSquareRoot => ScreenExpression.Last() == SQUARE_ROOT_CHAR;
	private bool IsCloseBracket => ScreenExpression.Last() == CLOSE_BRACKET;

	#region Property binding

	private string _screenExpression;
	public string ScreenExpression
	{
		get => _screenExpression;
		set { _screenExpression = value; OnPropertyChanged(); }
	}

	private string _screenResult;
	public string ScreenResult
	{
		get => _screenResult;
		set { _screenResult = value; OnPropertyChanged(); }
	}

	#endregion Property binding

	public ICommand LoadedWindowCommandAsync { get; private set; }
	public ICommand ClearScreenCommand { get; private set; }
	public ICommand AddNumberCommand { get; private set; }
	public ICommand AddOperationCommand { get; private set; }
	public ICommand AddMinusOperationCommand { get; private set; }
	public ICommand AddDecimalPointCommand { get; private set; }
	public ICommand AddSquareRootCommand { get; private set; }
	public ICommand AddOpenBracketCommand { get; private set; }
	public ICommand AddCloseBracketCommand { get; private set; }
	public ICommand GetResultCommand { get; private set; }
	public ICommand DeleteLastCharCommand { get; private set; }
	public ICommand ChangeCalculationMethodCommand { get; private set; }
	public ICommand ShowHistoryCommand { get; private set; }
	public ICommand ShowSettingsCommand { get; private set; }

	private void SetCommands()
	{
		LoadedWindowCommandAsync = new RelayCommandAsync(LoadedWindowAsync);
		ClearScreenCommand = new RelayCommand(ClearScreen);
		AddNumberCommand = new RelayCommand(AddNumber, CanAddNumber);
		AddOperationCommand = new RelayCommand(AddOperation, CanAddOperation);
		AddMinusOperationCommand = new RelayCommand(AddOperation, CanAddMinusOperation);
		AddDecimalPointCommand = new RelayCommand(AddDecimalPoint, CanAddDecimalPoint);
		AddSquareRootCommand = new RelayCommand(AddSquareRoot, CanAddSquareRoot);
		AddOpenBracketCommand = new RelayCommand(AddOpenBracket, CanAddOpenBracket);
		AddCloseBracketCommand = new RelayCommand(AddCloseBracket, CanAddCloseBracket);
		GetResultCommand = new RelayCommandAsync(GetResultAsync, CanGetResult);
		DeleteLastCharCommand = new RelayCommand(DeleteLastChar, CanDeleteLastChar);
		ChangeCalculationMethodCommand = new RelayCommand(ChangeCalculationMethod);
		ShowHistoryCommand = new RelayCommand(ShowHistory);
		ShowSettingsCommand = new RelayCommand(ShowSettings);
	}

	private async Task LoadedWindowAsync(object commandParameter)
	{
		if (!await ResultRepository.CheckConnectionAsync())
		{
			throw new InvalidOperationException("Database error!");
		}
	}

	private void ClearScreen(object commandParameter) => SetStartState();

	private void AddNumber(object commandParameter)
	{
		ClearExpressionIfAppInStartOrResultState();
		string number = commandParameter.ToString() ?? string.Empty;
		ScreenExpression += (number == "ANS") ? ScreenResult : number;
		SetNumberState();
	}

	private bool CanAddNumber(object obj) => !IsCloseBracket;

	private void AddOperation(object commandParameter)
	{
		if (_isResultDisplayed)
		{
			ScreenExpression = ScreenResult;
		}

		if (_isOperation && !_isMinusNumber)
		{
			_isMinusNumber = true;
		}

		ScreenExpression += commandParameter.ToString();
		SetOperationState();
	}

	private bool CanAddOperation(object commandParameter) => !_isOperation && !IsSeparator;

	private bool CanAddMinusOperation(object commandParameter) => !_isMinusNumber && !IsSeparator && !IsSquareRoot;

	private void AddDecimalPoint(object commandParameter)
	{
		if (ScreenExpression is EXPRESSION_START_CHAR || _isResultDisplayed)
		{
			ScreenExpression = "0";
		}

		if (_isOperation)
		{
			ScreenExpression += "0";
		}

		ScreenExpression += _separator;
		SetDecimalState();
	}

	private bool CanAddDecimalPoint(object commandParameter) => !_isDecimalPointInNumber && !IsCloseBracket;

	private void AddSquareRoot(object commandParameter)
	{
		ClearExpressionIfAppInStartOrResultState();
		ScreenExpression += SQUARE_ROOT_CHAR;
		AddScreenResultToScreenExpression();
		SetSquareRootState();
	}

	private bool CanAddSquareRoot(object commandParameter) => !_isSquareRootInNumber && (_isOperation || _isResultDisplayed);

	private void AddOpenBracket(object commandParameter)
	{
		ClearExpressionIfAppInStartOrResultState();
		ScreenExpression += OPEN_BRACKET;
		_openBracket++;
		AddScreenResultToScreenExpression();
		SetOpenBracketState();
	}

	private bool CanAddOpenBracket(object commandParameter) => _isOperation || _isResultDisplayed && !IsSeparator;

	private void AddCloseBracket(object commandParameter)
	{
		ScreenExpression += CLOSE_BRACKET;
		_closeBracket++;
	}

	private bool CanAddCloseBracket(object commandParameter) => _openBracket > _closeBracket && !_isOperation && !IsSeparator;

	private async Task GetResultAsync(object commandParameter)
	{
		try
		{
			string result = RemoveTrailingChars(CalculateExpression(), '0', _separator);
			await ResultRepository.AddResultAsync(new Result { Expression = ScreenExpression, Value = result, SaveDate = DateTime.Now });
			SetResultState(result);
		}
		catch (Exception ex)
		{
			System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
			SetStartState();
		}
	}

	private string RemoveTrailingChars(string text, params char[] chars) => text.Contains(_separator) ? text.TrimEnd(chars) : text;

	private bool CanGetResult(object commandParameter) => !_isOperation && !_isResultDisplayed && (_openBracket == _closeBracket);

	private void DeleteLastChar(object commandParameter)
	{
		int charsToRemove = 1;
		char lastChar = ScreenExpression.Last();

		if (lastChar == OPEN_BRACKET)
		{
			_openBracket--;
		}
		else if (lastChar == CLOSE_BRACKET)
		{
			_closeBracket--;
		}
		else if (lastChar == SQUARE_ROOT_CHAR)
		{
			_isSquareRootInNumber = false;
		}
		else if (lastChar == _separator)
		{
			if (ScreenExpression[^2] == '0' && (ScreenExpression.Length == 2 || _availableOperations.Contains(ScreenExpression[^3]) || ScreenExpression[^3] == OPEN_BRACKET))
			{
				charsToRemove = 2;
			}
			_isDecimalPointInNumber = false;
		}
		else if (_availableOperations.Contains(lastChar) && ScreenExpression.Length > 2 && (_availableOperations.Contains(ScreenExpression[^2]) || ScreenExpression[^2] == OPEN_BRACKET))
		{
			_isMinusNumber = false;
		}

		ScreenExpression = ScreenExpression.Remove(ScreenExpression.Length - charsToRemove);
		CheckExpressionAndSetAppState();
	}

	private bool CanDeleteLastChar(object commandParameter) => _canDeleteLastChar;

	private void ChangeCalculationMethod(object commandParameter)
	{
		if (commandParameter is Key key)
		{
			_calculationMethod = key switch
			{
				Key.D1 => _dataTableMethod,
				Key.D2 => _expressionParserMethod,
				_ => _infixToPostfixMethod
			};
			string? calculationMethodName = _calculationMethod.ToString();
			calculationMethodName = string.IsNullOrWhiteSpace(calculationMethodName) ? string.Empty : calculationMethodName[(calculationMethodName.LastIndexOf('.') + 1)..];
			System.Windows.MessageBox.Show($"New calculation method: {calculationMethodName}", "Change calculation method", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
		}
	}

	private void ShowHistory(object commandParameter)
	{
		HistoryView historyView = new();
		historyView.ShowDialog();
		Result result = historyView.HistoryViewModel.SelectedResult;

		if (result != null)
		{
			if (_isOperation)
			{
				ScreenExpression += result.Value;
			}
			else
			{
				ScreenExpression = result.Value;
			}

			CheckIfNumberContainDecimalPointOrSquareRoot();
			SetNumberState();
		}
	}

	private void ShowSettings(object obj)
	{
		// TODO
	}

	private void CheckExpressionAndSetAppState()
	{
		if (ScreenExpression.Length == 0)
		{
			SetStartState(false);
			return;
		}

		char lastChar = ScreenExpression.Last();

		if (char.IsDigit(lastChar))
		{
			CheckIfNumberContainDecimalPointOrSquareRoot();
		}
		else if (lastChar == _separator)
		{
			SetDecimalState();
		}
		else if (lastChar == SQUARE_ROOT_CHAR)
		{
			SetSquareRootState();
		}
		else if (lastChar == OPEN_BRACKET)
		{
			SetOpenBracketState();
		}
	}

	private void CheckIfNumberContainDecimalPointOrSquareRoot()
	{
		for (int i = ScreenExpression.Length - 1; i >= 0; i--)
		{
			if (ScreenExpression[i] == _separator)
			{
				_isDecimalPointInNumber = true;
			}

			if (ScreenExpression[i] == SQUARE_ROOT_CHAR)
			{
				_isSquareRootInNumber = true;
			}

			if (_availableOperations.Contains(ScreenExpression[i]))
			{
				break;
			}
		}
	}

	private string CalculateExpression() => _calculationMethod.Calculate(ScreenExpression.Replace(_separator, US_DECIMAL_SEPARATOR)) ?? throw new InvalidOperationException("The given expression could not be evaluated");

	private void ClearExpressionIfAppInStartOrResultState()
	{
		if (ScreenExpression is EXPRESSION_START_CHAR or RESULT_START_CHAR || _isResultDisplayed)
		{
			ScreenExpression = string.Empty;
		}
	}

	private void AddScreenResultToScreenExpression()
	{
		if (IsTheResultDisplayedAndNotZero)
		{
			ScreenExpression += ScreenResult;
		}
	}

	#region Set Application States

	private void SetStartState(bool clearResult = true)
	{
		_isOperation = false;
		_isMinusNumber = false;
		_isResultDisplayed = true;
		_canDeleteLastChar = false;
		_isSquareRootInNumber = false;
		_isDecimalPointInNumber = false;

		ScreenExpression = EXPRESSION_START_CHAR;
		if (clearResult)
		{
			ScreenResult = RESULT_START_CHAR;
		}

		_openBracket = _closeBracket = 0;
	}

	private void SetNumberState()
	{
		_isOperation = false;
		_isMinusNumber = false;
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
	}

	private void SetOperationState()
	{
		_isOperation = true;
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
		_isSquareRootInNumber = false;
		_isDecimalPointInNumber = false;
	}

	private void SetDecimalState()
	{
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
		_isSquareRootInNumber = false;
		_isDecimalPointInNumber = true;
	}

	private void SetSquareRootState()
	{
		_isOperation = !IsTheResultDisplayedAndNotZero;
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
		_isSquareRootInNumber = true;
	}

	private void SetOpenBracketState()
	{
		_isOperation = !IsTheResultDisplayedAndNotZero;
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
		_isSquareRootInNumber = false;
		_isDecimalPointInNumber = false;
	}

	private void SetResultState(string result)
	{
		_isResultDisplayed = true;
		_canDeleteLastChar = false;
		_isSquareRootInNumber = false;
		_isDecimalPointInNumber = false;

		ScreenExpression += "=";
		ScreenResult = result;

		_openBracket = _closeBracket = 0;
	}

	#endregion Set Application States
}