namespace Calculator.WpfApp.Models.Calculation;

internal interface ICalculate
{
	/// <summary>
	/// Evaluates the given expression. The decimal separator is '.'
	/// </summary>
	/// <param name="expression"></param>
	/// <returns>Expression result or null.</returns>
	public string? Calculate(string expression);
}