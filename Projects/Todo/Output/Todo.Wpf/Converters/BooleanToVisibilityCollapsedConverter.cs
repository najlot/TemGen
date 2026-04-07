using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Todo.Wpf.Converters;

public class BooleanToVisibilityCollapsedConverter : IValueConverter
{
	public static BooleanToVisibilityCollapsedConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is true ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is Visibility.Collapsed;
	}
}
