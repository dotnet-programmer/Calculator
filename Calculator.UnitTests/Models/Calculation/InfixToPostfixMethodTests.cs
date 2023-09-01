using Calculator.WpfApp.Models.Calculation;
using NUnit.Framework;

namespace Calculator.UnitTests.Models.Calculation;

internal class InfixToPostfixMethodTests
{
	[Test]
	public void Calculate_WhenCalled_ShouldReturnResult()
	{
		InfixToPostfixMethod infixToPostfixMethod = new();
		var result = infixToPostfixMethod.Calculate("2+2*2");
		Assert.AreEqual("6", result);
	}
}