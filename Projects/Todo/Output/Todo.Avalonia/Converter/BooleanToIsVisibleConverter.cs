using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Todo.Avalonia.Converter;

public class BooleanToIsVisibleConverter : IValueConverter
{
	public static BooleanToIsVisibleConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is true;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is true;
	}
}

