/*
    Grammar used in this program to handle expressions:

	Expression:
        Term
        Expression "+" Term
        Expression "-" Term

    Term:
        Primary
        Term "*" Primary
        Term "/" Primary
        Term "%" Primary

    Primary:
        Number
        Name
        "(" Expression ")"
        "+" Primary
        "-" Primary
        "pow(" Primary "," Primary ")"
        "sqrt(" Primary ")"

    Number:
        floating-point-literal
 */

using System;

namespace Calculator.WpfApp.Models;

internal class ExpressionParser
{
	private string? _expression;
	private string? _separator;
	private int _position;
	private bool _isFull;
	private bool _isEndOfExpression;
	private Token? _buffer;

	public string Calculate(string expression, string separator)
	{
		_expression = expression;
		_separator = separator;
		_position = 0;
		_isFull = false;
		_isEndOfExpression = false;
		_buffer = null;
		return Expression().ToString();
	}

	private decimal Expression()
	{
		decimal left = Term();
		while (!_isEndOfExpression)
		{
			Token token = GetToken();
			switch (token.Value)
			{
				case "+":
					left += Term();
					break;
				case "-":
					left -= Term();
					break;
				default:
					PutbackToken(token);
					return left;
			}
		}
		return left;
	}

	private decimal Term()
	{
		decimal left = Primary();
		while (!_isEndOfExpression)
		{
			Token token = GetToken();
			switch (token.Value)
			{
				case "*":
					left *= Primary();
					break;
				case "/":
					decimal right = Primary();
					if (right == 0)
					{
						throw new DivideByZeroException("Divide by zero!");
					}
					left /= right;
					break;
				case "^":
					if (left == 0)
					{
						left = 1;
						break;
					}
					decimal pow = left;
					decimal rhsValue = Primary();
					for (int j = 1; j < rhsValue; j++)
					{
						left *= pow;
					}
					break;
				default:
					PutbackToken(token);
					return left;
			}
		}
		return left;
	}

	private decimal Primary()
	{
		Token token = GetToken();
		if (token.Value == "(")
		{
			decimal number = Expression();
			token = GetToken();
			return token.Value != ")" ? throw new InvalidOperationException("Wrong input - missing bracket!") : number;
		}
		else if (token.Value == "√")
		{
			token = GetToken();
			return (decimal)Math.Sqrt(double.Parse(token.Value));
		}
		else if (token.Value == "-")
		{
			return -Primary();
		}
		else
		{
			return token.TokenType == TokenType.Operand ? decimal.Parse(token.Value) : throw new ArgumentException("Unknown operator!");
		}
	}

	private Token GetToken()
	{
		if (_isFull)
		{
			_isFull = false;
			return _buffer;
		}

		if (_position == _expression.Length - 1)
		{
			_isEndOfExpression = true;
		}

		char actualChar = _expression[_position];
		if (char.IsDigit(actualChar) || actualChar == _separator[0])
		{
			string number = string.Empty;
			while (_position < _expression.Length && (char.IsDigit(_expression[_position]) || _expression[_position] == _separator[0]))
			{
				number += _expression[_position];
				_position++;
			}
			return new(TokenType.Operand, number);
		}
		else
		{
			_position++;
			return new(TokenType.Operator, actualChar);
		}
	}

	private void PutbackToken(Token token)
	{
		_buffer = token;
		_isFull = true;
	}
}