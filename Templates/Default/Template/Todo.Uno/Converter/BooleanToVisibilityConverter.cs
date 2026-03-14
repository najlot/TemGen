using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace <#cs Write(Project.Namespace)#>.Uno.Converter;

public class BooleanToVisibilityConverter : IValueConverter
{
	public static BooleanToVisibilityConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, string language)
	{
		return value is true ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, string language)
	{
		return value is Visibility.Visible;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
