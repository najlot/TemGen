using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace <#cs Write(Project.Namespace)#>.Uno.Converter;

public class BooleanToVisibilityCollapsedConverter : IValueConverter
{
	public static BooleanToVisibilityCollapsedConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, string language)
	{
		return value is true ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, string language)
	{
		return value is Visibility.Collapsed;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
