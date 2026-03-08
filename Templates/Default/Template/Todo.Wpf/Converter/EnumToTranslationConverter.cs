using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace <#cs Write(Project.Namespace)#>.Wpf.Converter;

public class EnumToTranslationConverter : IValueConverter
{
	public static IValueConverter Instance { get; } = new EnumToTranslationConverter();

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is null)
		{
			return string.Empty;
		}

		if (parameter is not Type type)
		{
			return string.Empty;
		}

		var resourceManager = new System.Resources.ResourceManager(type);

		if (value is ICollection collection)
		{
			var newCollection = new List<object>();

			foreach (var item in collection)
			{
				newCollection.Add(Translate((Enum)item, resourceManager));
			}

			return newCollection;
		}

		return Translate((Enum)value, resourceManager);
	}

	private static string Translate(Enum value, System.Resources.ResourceManager resourceManager)
	{
		if (value is null)
		{
			return string.Empty;
		}

		return resourceManager.GetString(value.ToString()) ?? string.Empty;
	}

	public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is null)
		{
			return null;
		}

		if (parameter is not Type type)
		{
			return null;
		}

		var resourceManager = new System.Resources.ResourceManager(type);

		if (targetType.IsConstructedGenericType)
		{
			if (value is ICollection collection)
			{
				var newCollection = new List<object>();

				foreach (var item in collection)
				{
					newCollection.Add(TranslateBack(item.ToString(), resourceManager, targetType) ?? string.Empty);
				}

				return newCollection;
			}

			return null;
		}

		return TranslateBack(value.ToString(), resourceManager, targetType);
	}

	private static object? TranslateBack(string? value, System.Resources.ResourceManager resourceManager, Type targetType)
	{
		var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

		if (resourceSet is null)
		{
			return null;
		}

		foreach (DictionaryEntry entry in resourceSet)
		{
			if (entry.Value?.ToString() == value)
			{
				return Enum.Parse(targetType, entry.Key.ToString() ?? string.Empty);
			}
		}

		return value;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
