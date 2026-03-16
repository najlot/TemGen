using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using <#cs Write(Project.Namespace)#>.Client.Localisation;

namespace <#cs Write(Project.Namespace)#>.Uno.Converter;

public class EnumToTranslationConverter : IValueConverter
{
	public static EnumToTranslationConverter Instance { get; } = new();

	public object? Convert(object? value, Type targetType, object? parameter, string language)
	{
		if (value is null)
		{
			return string.Empty;
		}

		var type = GetResourceType(parameter);

		if (type is null)
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

	public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
	{
		if (value is null)
		{
			return null;
		}

		var type = GetResourceType(parameter);

		if (type is null)
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

	private static Type? GetResourceType(object? parameter)
	{
		if (parameter is Type type)
		{
			return type;
		}

		if (parameter is string typeName && !string.IsNullOrWhiteSpace(typeName))
		{
			return typeName switch
			{
<#cs
foreach (var definition in Definitions.Where(d => d.IsEnumeration))
{
	WriteLine($"\t\t\t\t\"{definition.Name}Loc\" => typeof({definition.Name}Loc),");
}
#>
				_ => null,
			};
		}

		return null;
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
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
