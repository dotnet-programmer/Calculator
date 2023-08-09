using Calculator.WpfApp.Models.Domains;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calculator.WpfApp.Models.Converters;

public class HistoryViewParamsConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		=> new HistoryViewParams { Result = values[0] as Result, Window = values[1] as Window };

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}