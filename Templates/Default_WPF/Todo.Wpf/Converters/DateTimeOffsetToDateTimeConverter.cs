using System;
using System.Globalization;
using System.Windows.Data;

namespace <# Project.Namespace#>.Wpf.Converters;

public class DateTimeOffsetToDateTimeConverter : IValueConverter
{
	public static DateTimeOffsetToDateTimeConverter Instance { get; } = new();

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is DateTimeOffset dateTimeOffset ? dateTimeOffset.Date : null;

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is DateTime dateTime
			? new DateTimeOffset(DateTime.SpecifyKind(dateTime.Date, DateTimeKind.Unspecified), TimeSpan.Zero)
			: null;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>