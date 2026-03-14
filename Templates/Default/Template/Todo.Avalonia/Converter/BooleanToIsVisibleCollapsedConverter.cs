using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace <#cs Write(Project.Namespace)#>.Avalonia.Converter;

public class BooleanToIsVisibleCollapsedConverter : IValueConverter
{
	public static BooleanToIsVisibleCollapsedConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is not true;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is not true;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
