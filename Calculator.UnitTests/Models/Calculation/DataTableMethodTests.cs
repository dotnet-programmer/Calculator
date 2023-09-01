using Calculator.WpfApp.Models.Calculation;
using FluentAssertions;
using NUnit.Framework;

namespace Calculator.UnitTests.Models.Calculation;

internal class DataTableMethodTests
{
	private DataTableMethod _dataTableMethod;

	[SetUp]
	public void SetUp() => _dataTableMethod = new();

	[TestCase("1+2", "3")]
	[TestCase("0-1+-2", "-3")]
	[TestCase("1-2", "-1")]
	public void Calculate_WhenCalled_ShouldReturnResult(string expression, string expectedResult)
	{
		var result = _dataTableMethod.Calculate(expression);
		result.Should().Be(expectedResult);
	}
}