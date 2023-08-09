using Calculator.WpfApp.Commands;
using Calculator.WpfApp.Models;
using Calculator.WpfApp.Models.Domains;
using Calculator.WpfApp.Repositories;
using Calculator.WpfApp.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.WpfApp.ViewModels;

internal enum CalculationMethod
{ DataTable, InfixToPostfix, ExpressionParser }

internal class MainViewModel : BaseViewModel
{
	private CalculationMethod _calculationMethod;
	private readonly DataTable _dataTable = new();
	private readonly ExpressionParser _expressionParser = new();
	private bool _isOperation;
	private bool _isDecimalPointInNumber;
	private bool _isSquareRootInNumber;
	private bool _isResultDisplayed;
	private bool _isMinusNumber;
	private bool _canDeleteLastChar;
	private const string _expressionStartChar = "_";
	private const string _resultStartChar = "0";
	private const string _squareRootChar = "√";
	private const string _powerChar = "^";
	private readonly string _separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
	private readonly List<char> _availableOperations = new() { '+', '-', '*', '/', '^' };

	public MainViewModel()
	{
		_isResultDisplayed = true;
		_calculationMethod = CalculationMethod.InfixToPostfix;
		ScreenExpression = _expressionStartChar;
		ScreenResult = _resultStartChar;
		SetCommands();
	}

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

	public ICommand LoadedWindowCommandAsync { get; private set; }
	public ICommand AddNumberCommand { get; private set; }
	public ICommand AddOperationCommand { get; private set; }
	public ICommand AddMinusOperationCommand { get; private set; }
	public ICommand ClearScreenCommand { get; private set; }
	public ICommand GetResultCommandAsync { get; private set; }
	public ICommand AddDecimalPointCommand { get; private set; }
	public ICommand DeleteLastCharCommand { get; private set; }
	public ICommand ShowHistoryCommand { get; private set; }
	public ICommand AddSquareRootCommand { get; private set; }
	public ICommand AddBracketCommand { get; private set; }
	public ICommand ChangeCalculationMethodCommand { get; private set; }

	private void SetCommands()
	{
		LoadedWindowCommandAsync = new RelayCommandAsync(LoadedWindowAsync);
		AddNumberCommand = new RelayCommand(AddNumber);
		AddOperationCommand = new RelayCommand(AddOperation, CanAddOperation);
		AddMinusOperationCommand = new RelayCommand(AddOperation, CanAddMinusOperation);
		ClearScreenCommand = new RelayCommand(ClearScreen);
		GetResultCommandAsync = new RelayCommandAsync(GetResultAsync, CanGetResult);
		AddDecimalPointCommand = new RelayCommand(AddDecimalPoint, CanAddDecimalPoint);
		DeleteLastCharCommand = new RelayCommand(DeleteLastChar, CanDeleteLastChar);
		ShowHistoryCommand = new RelayCommand(ShowHistory);
		AddSquareRootCommand = new RelayCommand(AddSquareRoot, CanAddSquareRoot);
		AddBracketCommand = new RelayCommand(AddBracket);
		ChangeCalculationMethodCommand = new RelayCommand(ChangeCalculationMethod);
	}

	private async Task LoadedWindowAsync(object obj)
	{
		if (!await ResultRepository.CheckConnectionAsync())
		{
			throw new InvalidOperationException("Błąd pliku bazy danych!");
		}
	}

	private void ClearScreen(object obj)
	{
		ScreenExpression = _expressionStartChar;
		ScreenResult = _resultStartChar;

		_isDecimalPointInNumber = false;
		_isOperation = false;
		_canDeleteLastChar = false;
		_isSquareRootInNumber = false;
		_isResultDisplayed = true;
	}

	private void AddNumber(object obj)
	{
		if (ScreenExpression == _expressionStartChar || ScreenExpression == _resultStartChar || _isResultDisplayed)
		{
			ScreenExpression = string.Empty;
		}

		if (obj is Key key)
		{
			obj = ConvertKeyToChar(key);
		}
		else if (obj is Tuple<object, object> tuple)
		{
			if ((Key)tuple.Item1 == Key.D9)
			{
				obj = '(';
			}
			else if ((Key)tuple.Item1 == Key.D0)
			{
				obj = ')';
			}
		}

		ScreenExpression += obj.ToString();

		_isOperation = false;
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
		_isMinusNumber = false;
	}

	private void AddOperation(object obj)
	{
		if (_isResultDisplayed)
		{
			ScreenExpression = ScreenResult;
		}

		if (obj is Key key)
		{
			if (key == Key.D6)
			{
				obj = _powerChar;
			}
			else
			{
				if (key == Key.D8)
				{
					key = Key.Multiply;
				}
				obj = ConvertKeyToChar(key);
			}
		}

		if (_isOperation && !_isMinusNumber)
		{
			_isMinusNumber = true;
		}

		ScreenExpression += obj.ToString();

		_isOperation = true;
		_isDecimalPointInNumber = false;
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
		_isSquareRootInNumber = false;
	}

	private bool CanAddOperation(object obj)
		=> !_isOperation;

	private bool CanAddMinusOperation(object obj) => !_isMinusNumber;

	private void AddDecimalPoint(object obj)
	{
		if (_isResultDisplayed || ScreenExpression == _expressionStartChar)
		{
			ScreenExpression = "0";
		}

		if (_isOperation)
		{
			ScreenExpression += "0";
		}

		ScreenExpression += ",";

		_isDecimalPointInNumber = true;
		_isResultDisplayed = false;
		_isSquareRootInNumber = false;
		_canDeleteLastChar = true;
	}

	private bool CanAddDecimalPoint(object obj)
			=> !_isDecimalPointInNumber;

	private void AddSquareRoot(object obj)
	{
		if (ScreenExpression is _expressionStartChar or _resultStartChar)
		{
			ScreenExpression = _squareRootChar;
		}
		else if (_isResultDisplayed)
		{
			ScreenExpression = _squareRootChar + ScreenResult;
		}
		else if (_isOperation)
		{
			ScreenExpression += _squareRootChar;
		}

		_isSquareRootInNumber = true;
		_isOperation = true;
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
		_isMinusNumber = true;
	}

	private bool CanAddSquareRoot(object obj)
		=> !_isSquareRootInNumber && (_isOperation || _isResultDisplayed);

	private static char ConvertKeyToChar(Key key)
	{
		// return value: https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=netframework-4.8
		int keyCode = KeyInterop.VirtualKeyFromKey(key);

		if (keyCode is >= 48 and <= 57)
		{
			// numbers 0-9
			return (char)keyCode;
		}
		else if (keyCode is >= 96 and <= 105)
		{
			// Numpad keys 0-9
			return (char)(keyCode - 48);
		}
		else if (keyCode is 187 or 189 or 191)
		{
			// keys + - /
			return (char)(keyCode - 144);
		}
		else
		{
			// Numpad keys + - * /
			return (char)(keyCode - 64);
		}
	}

	private void AddBracket(object obj)
	{
		if (ScreenExpression is _expressionStartChar or _resultStartChar)
		{
			ScreenExpression = string.Empty;
		}

		if (obj is Key key)
		{
			if (key == Key.D9)
			{
				obj = '(';
			}
			else if (key == Key.D0)
			{
				obj = ')';
			}
		}

		string text = obj.ToString();
		if (text == "(")
		{
			_isOperation = true;
		}

		ScreenExpression += text;

		if (_isResultDisplayed && ScreenResult != "0")
		{
			ScreenExpression += ScreenResult;
		}

		_isDecimalPointInNumber = false;
		_isResultDisplayed = false;
		_canDeleteLastChar = true;
		_isSquareRootInNumber = false;
	}

	private async Task GetResultAsync(object obj)
	{
		if (_isResultDisplayed)
		{
			return;
		}

		try
		{
			string? result = CalculateExpression();

			if (result.Contains(_separator))
			{
				result = result.TrimEnd('0', _separator[0]);
			}

			ScreenResult = result;

			_isResultDisplayed = true;
			_isDecimalPointInNumber = false;
			_canDeleteLastChar = false;
			_isSquareRootInNumber = false;

			await ResultRepository.AddResultAsync(new() { Expression = ScreenExpression, Value = ScreenResult, SaveDate = DateTime.Now });
			ScreenExpression += "=";
		}
		catch (Exception ex)
		{
			ScreenResult = ex.Message;
		}
	}

	private string CalculateExpression()
	{
		switch (_calculationMethod)
		{
			case CalculationMethod.DataTable:
				return _dataTable.Compute(ScreenExpression.Replace(_separator, "."), null).ToString();
			case CalculationMethod.InfixToPostfix:
				return Calculation.Calculate(ScreenExpression, _separator);
			case CalculationMethod.ExpressionParser:
				return _expressionParser.Calculate(ScreenExpression, _separator);
			default:
				throw new InvalidOperationException("Invalid calculation method!");
		}
	}

	private bool CanGetResult(object obj)
		=> !_isOperation;

	private void DeleteLastChar(object obj)
	{
		int charsToRemove = 1;

		if (ScreenExpression.Last().ToString() == _separator)
		{
			// delete "0,"
			if (ScreenExpression[^2] == '0' && (ScreenExpression.Length == 2 || _availableOperations.Contains(ScreenExpression[^3])))
			{
				charsToRemove = 2;
			}
			_isDecimalPointInNumber = false;
		}
		else if (ScreenExpression.Last().ToString() == _squareRootChar)
		{
			_isSquareRootInNumber = false;
		}

		ScreenExpression = ScreenExpression.Remove(ScreenExpression.Length - charsToRemove);

		if (ScreenExpression.Length == 0)
		{
			_canDeleteLastChar = false;
			ScreenExpression = _expressionStartChar;
		}

		_isOperation = _availableOperations.Contains(ScreenExpression.Last());

		_isMinusNumber = _isOperation && _availableOperations.Contains(ScreenExpression[^2]) || ScreenExpression[^2] == '(';

		if (!_isOperation)
		{
			CheckDecimalPointAndSquareRoot();
		}
	}

	private void CheckDecimalPointAndSquareRoot()
	{
		for (int i = ScreenExpression.Length - 1; i >= 0; i--)
		{
			if (ScreenExpression[i].ToString() == _separator)
			{
				_isDecimalPointInNumber = true;
			}

			if (ScreenExpression[i].ToString() == _squareRootChar)
			{
				_isSquareRootInNumber = true;
			}

			if (_availableOperations.Contains(ScreenExpression[i]))
			{
				break;
			}
		}
	}

	private bool CanDeleteLastChar(object obj)
		=> _canDeleteLastChar;

	private void ShowHistory(object obj)
	{
		var historyView = new HistoryView();
		historyView.ShowDialog();
		Result result = historyView.HistoryViewModel.HistoryResult;

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

			CheckDecimalPointAndSquareRoot();

			_isOperation = false;
			_canDeleteLastChar = true;
			_isResultDisplayed = false;
		}
	}

	private void ChangeCalculationMethod(object obj)
	{
		if (obj is Key key)
		{
			switch (key)
			{
				case Key.D1:
					_calculationMethod = CalculationMethod.DataTable;
					break;
				case Key.D2:
					_calculationMethod = CalculationMethod.InfixToPostfix;
					break;
				case Key.D3:
					_calculationMethod = CalculationMethod.ExpressionParser;
					break;
			}
			ScreenResult = _calculationMethod.ToString();
		}
	}
}