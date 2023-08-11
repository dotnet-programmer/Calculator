// Algorithm:
// https://www.geeksforgeeks.org/convert-infix-expression-to-postfix-expression/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.WpfApp.Models.Calculation;

internal class InfixToPostfixMethod : ICalculate
{
	private const char SEPARATOR = '.';
	private bool _isMinusNumber;

	public string Calculate(string expression) => PostfixEvaluator(InfixToPostfix(expression));

	private List<Token> InfixToPostfix(string expression)
	{
		Stack<char> operators = new();
		List<Token> tokensInPostfixNotation = new();
		Dictionary<char, int> operatorsPrecedenceMap = new()
		{
			['('] = 0,
			['+'] = 1,
			['-'] = 1,
			['*'] = 2,
			['/'] = 2,
			['^'] = 3,
		};

		int i = 0;
		while (i < expression.Length)
		{
			char actualChar = expression[i];
			if (char.IsDigit(actualChar) || actualChar == SEPARATOR)
			{
				string number = MakeNumber();
				tokensInPostfixNotation.Add(new(TokenType.Operand, number));
			}
			else
			{
				i++;

				if (actualChar == '(')
				{
					operators.Push(actualChar);
					CheckMinusNumber();
				}
				else if (actualChar == ')')
				{
					while (operators.Peek() != '(')
					{
						AddOperatorToTokens(operators.Pop());
						if (operators.Count == 0)
						{
							throw new InvalidOperationException("Wrong input - missing bracket!");
						}
					}
					_ = operators.Pop();
				}
				else if (operatorsPrecedenceMap.ContainsKey(actualChar))
				{
					while (operators.Any() && operatorsPrecedenceMap[operators.Peek()] >= operatorsPrecedenceMap[actualChar])
					{
						AddOperatorToTokens(operators.Pop());
					}
					operators.Push(actualChar);
					CheckMinusNumber();
				}
				else if (actualChar == '√')
				{
					string number = MakeNumber();
					number = Math.Sqrt(double.Parse(number)).ToString();
					tokensInPostfixNotation.Add(new(TokenType.Operand, number));
				}
				else
				{
					throw new ArgumentException("Unknown operator!");
				}
			}
		}

		while (operators.Any())
		{
			AddOperatorToTokens(operators.Pop());
		}

		return tokensInPostfixNotation;

		void AddOperatorToTokens(char operatorToAdd) => tokensInPostfixNotation.Add(new(TokenType.Operator, operatorToAdd));

		string MakeNumber()
		{
			string number = string.Empty;
			if (_isMinusNumber)
			{
				number = "-";
				_isMinusNumber = false;
			}
			while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == SEPARATOR))
			{
				number += expression[i];
				i++;
			}
			return number;
		}

		void CheckMinusNumber()
		{
			if (expression[i] == '-')
			{
				_isMinusNumber = true;
				i++;
			}
		}
	}

	private string PostfixEvaluator(List<Token> tokens)
	{
		Stack<decimal> resultStack = new();

		for (int i = 0; i < tokens.Count; i++)
		{
			Token actualToken = tokens[i];

			if (actualToken.TokenType == TokenType.Operand)
			{
				resultStack.Push(decimal.Parse(actualToken.Value));
			}
			else
			{
				decimal rhsValue = resultStack.Pop();
				decimal lhsValue = resultStack.Pop();
				decimal result;
				switch (actualToken.Value[0])
				{
					case '+':
						result = lhsValue + rhsValue;
						break;
					case '-':
						result = lhsValue - rhsValue;
						break;
					case '*':
						result = lhsValue * rhsValue;
						break;
					case '/':
						if (rhsValue == 0)
						{
							throw new DivideByZeroException("Divide by zero!");
						}
						result = lhsValue / rhsValue;
						break;
					case '^':
						if (rhsValue == 0)
						{
							result = 1;
							break;
						}
						result = lhsValue;
						for (int j = 1; j < rhsValue; j++)
						{
							result *= lhsValue;
						}
						break;
					default:
						throw new ArgumentException("Unknown operator!");
				}
				resultStack.Push(result);
			}
		}
		return resultStack.Pop().ToString();
	}
}