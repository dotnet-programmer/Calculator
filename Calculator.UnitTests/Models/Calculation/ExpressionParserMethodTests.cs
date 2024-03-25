using Calculator.WpfApp.Models.Calculation;
using FluentAssertions;
using NUnit.Framework;

namespace Calculator.UnitTests.Models.Calculation;

internal class ExpressionParserMethodTests
{
	private ExpressionParserMethod _expressionParserMethod;

	[SetUp]
	public void SetUp()
		=> _expressionParserMethod = new();

	[TestCase("1+2", "3")]
	[TestCase("0-1+-2", "-3")]
	[TestCase("1-2", "-1")]
	public void Calculate_WhenCalled_ShouldReturnResult(string expression, string expectedResult)
	{
		var result = _expressionParserMethod.Calculate(expression);
		result.Should().Be(expectedResult);
	}
}