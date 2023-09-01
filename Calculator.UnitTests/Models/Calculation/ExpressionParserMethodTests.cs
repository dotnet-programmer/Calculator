using Calculator.WpfApp.Models.Calculation;
using NUnit.Framework;

namespace Calculator.UnitTests.Models.Calculation;

internal class ExpressionParserMethodTests
{
	[Test]
	public void Calculate_WhenCalled_ShouldReturnResult()
	{
		ExpressionParserMethod expressionParserMethod = new();
		var result = expressionParserMethod.Calculate("2+2*2");
		Assert.AreEqual("6", result);
	}
}