using System;

namespace Calculator.WpfApp.Models.Domains;

public class Result
{
	public int ResultId { get; set; }
	public string? Expression { get; set; }
	public string? Value { get; set; }
	public DateTime SaveDate { get; set; }
}