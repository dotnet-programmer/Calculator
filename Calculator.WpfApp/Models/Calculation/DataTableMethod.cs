using System.Data;

namespace Calculator.WpfApp.Models.Calculation;

public class DataTableMethod : ICalculate
{
	private readonly DataTable _dataTable = new();

	public string? Calculate(string expression) 
		=> _dataTable.Compute(expression, null).ToString();
}