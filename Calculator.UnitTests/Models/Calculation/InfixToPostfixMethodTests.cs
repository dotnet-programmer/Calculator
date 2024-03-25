using Calculator.WpfApp.Models.Calculation;
using FluentAssertions;
using NUnit.Framework;

namespace Calculator.UnitTests.Models.Calculation;

internal class InfixToPostfixMethodTests
{
	private InfixToPostfixMethod _infixToPostfixMethod;

	[SetUp]
	public void SetUp()
		=> _infixToPostfixMethod = new();

	[TestCase("1+2", "3")]
	[TestCase("0-1+-2", "-3")]
	[TestCase("1-2", "-1")]
	public void Calculate_WhenCalled_ShouldReturnResult(string expression, string expectedResult)
	{
		var result = _infixToPostfixMethod.Calculate(expression);
		result.Should().Be(expectedResult);
	}
}