using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace <# Project.Namespace#>.Wpf.Converters;

public class EnumToTranslationConverter : IValueConverter
{
	public static EnumToTranslationConverter Instance { get; } = new();

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
		if (parameter is not Type type)
		{
			return Binding.DoNothing;
		}

		var resourceManager = new System.Resources.ResourceManager(type);
		var emptySelectionValue = GetEmptySelectionValue(targetType);

		if (value is null)
		{
			return emptySelectionValue;
		}

		if (targetType.IsConstructedGenericType)
		{
			if (value is ICollection collection)
			{
				var newCollection = new List<object>();

				foreach (var item in collection)
				{
					newCollection.Add(TranslateBack(item?.ToString(), resourceManager, targetType, emptySelectionValue));
				}

				return newCollection;
			}

			return Binding.DoNothing;
		}

		return TranslateBack(value.ToString(), resourceManager, targetType, emptySelectionValue);
	}

	private static object TranslateBack(string? value, System.Resources.ResourceManager resourceManager, Type targetType, object emptySelectionValue)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return emptySelectionValue;
		}

		var enumType = Nullable.GetUnderlyingType(targetType) ?? targetType;
		var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

		if (resourceSet is null)
		{
			return emptySelectionValue;
		}

		foreach (DictionaryEntry entry in resourceSet)
		{
			if (entry.Value?.ToString() == value)
			{
				return Enum.Parse(enumType, entry.Key.ToString() ?? string.Empty);
			}
		}

		if (Enum.TryParse(enumType, value, true, out var enumValue))
		{
			return enumValue;
		}

		return emptySelectionValue;
	}

	private static object GetEmptySelectionValue(Type targetType)
	{
		var enumType = Nullable.GetUnderlyingType(targetType) ?? targetType;

		if (enumType.IsEnum)
		{
			return Enum.ToObject(enumType, 0);
		}

		return Binding.DoNothing;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
