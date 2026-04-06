using System;
using System.Globalization;
using System.Windows.Data;

namespace <# Project.Namespace#>.Wpf.Converters;

public class NullableTimeSpanConverter : IValueConverter
{
	public static NullableTimeSpanConverter Instance { get; } = new();

	private static readonly string[] Formats = ["hh\\:mm", "h\\:mm", "hh\\:mm\\:ss", "h\\:mm\\:ss"];

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is TimeSpan timeSpan ? timeSpan.ToString(@"hh\:mm", culture) : string.Empty;

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not string text || string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		if (TimeSpan.TryParseExact(text.Trim(), Formats, culture, out var parsed)
			|| TimeSpan.TryParse(text.Trim(), culture, out parsed))
		{
			return parsed;
		}

		return Binding.DoNothing;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>