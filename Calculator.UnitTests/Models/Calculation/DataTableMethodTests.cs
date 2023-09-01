using Calculator.WpfApp.Models.Calculation;
using NUnit.Framework;

namespace Calculator.UnitTests.Models.Calculation;

internal class DataTableMethodTests
{
	[Test]
	public void Calculate_WhenCalled_ShouldReturnResult()
	{
		DataTableMethod dataTableMethod = new();
		var result = dataTableMethod.Calculate("2+2*2");
		Assert.AreEqual("6", result);
	}
}