using System.Windows;
using Calculator.WpfApp.Models.Domains;

namespace Calculator.WpfApp.Models;

internal class HistoryViewParams
{
	public Result Result { get; set; }
	public Window Window { get; set; }
}