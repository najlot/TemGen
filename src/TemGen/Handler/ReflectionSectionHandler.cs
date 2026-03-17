using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class ReflectionSectionHandler() : AbstractSectionHandler(TemplateHandler.Reflection)
{
	private static string GetValue(IEnumerable<string> parts, object obj)
	{
		if (obj is null)
		{
			return "";
		}

		if (!parts.Any())
		{
			return obj.ToString() ?? "";
		}

		var type = obj.GetType();
		var propertyName = parts.First();

		if (string.IsNullOrEmpty(propertyName))
		{
			throw new Exception("Property name is null or empty. Expression ends with '.'?");
		}

		var property = type.GetProperty(propertyName);

		if (property is null)
		{
			if (obj is Globals globals)
			{
				var variableValue = globals.GetVariable(propertyName);

				if (variableValue is not null)
				{
					return GetValue(parts.Skip(1), variableValue);
				}
			}

			if (obj is Project project
				&& project.TryGetSetting(propertyName, out var projectSettingValue))
			{
				return GetValue(parts.Skip(1), projectSettingValue);
			}

			if (obj is IReadOnlyDictionary<string, string> readOnlyStringDictionary
				&& readOnlyStringDictionary.TryGetValue(propertyName, out var readOnlyDictionaryValue))
			{
				return GetValue(parts.Skip(1), readOnlyDictionaryValue);
			}

			if (obj is IDictionary<string, string> stringDictionary
				&& stringDictionary.TryGetValue(propertyName, out var dictionaryValue))
			{
				return GetValue(parts.Skip(1), dictionaryValue);
			}

			throw new Exception("Property '" + propertyName + "' not found.");
		}

		obj = property.GetValue(obj, null);

		return GetValue(parts.Skip(1), obj);
	}

	protected override Task Handle(Globals globals, string content)
	{
		if (string.IsNullOrWhiteSpace(content))
		{
			return Task.CompletedTask;
		}

		var parts = content.Trim().Split('.');
		var value = GetValue(parts, globals);

		globals.Write(value);
		return Task.CompletedTask;
	}
}