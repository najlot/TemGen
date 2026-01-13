using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace <#cs Write(Project.Namespace)#>.Wpf.Converter;

public class EnumToTranslationConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}

		var resourceManager = new System.Resources.ResourceManager(parameter as Type);

		if (value.GetType().IsConstructedGenericType)
		{
			var collection = value as ICollection;
			var newCollection = new List<object>();

			foreach (var item in collection)
			{
				newCollection.Add(Translate((Enum)item, resourceManager));
			}

			return newCollection;
		}

		return Translate((Enum)value, resourceManager);
	}

	private string Translate(Enum value, System.Resources.ResourceManager resourceManager)
	{
		if (value == null)
		{
			return "";
		}

		return resourceManager.GetString(value.ToString());
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}

		var resourceManager = new System.Resources.ResourceManager(parameter as Type);

		if (targetType.IsConstructedGenericType)
		{
			if (value is ICollection collection)
			{
				var newCollection = new List<object>();

				foreach (var item in collection)
				{
					newCollection.Add(TranslateBack(item.ToString(), resourceManager, targetType));
				}

				return newCollection;
			}

			return null;
		}

		return TranslateBack(value.ToString(), resourceManager, targetType);
	}

	private object TranslateBack(string value, System.Resources.ResourceManager resourceManager, Type targetType)
	{
		var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

		foreach (DictionaryEntry entry in resourceSet)
		{
			if (entry.Value.ToString() == value)
			{
				return Enum.Parse(targetType, entry.Key.ToString());
			}
		}

		return value;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>