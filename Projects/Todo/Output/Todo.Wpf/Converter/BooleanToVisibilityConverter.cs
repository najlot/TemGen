using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Todo.Wpf.Converter;

public class BooleanToVisibilityConverter : IValueConverter
{
	public static BooleanToVisibilityConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is true ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is Visibility.Visible;
	}
}
