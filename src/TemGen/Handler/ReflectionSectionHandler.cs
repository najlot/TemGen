using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemGen.Handler;

public sealed class ReflectionSectionHandler : AbstractSectionHandler
{
	private string GetValue(IEnumerable<string> parts, object obj)
	{
		if (obj is null)
		{
			return "";
		}

		if (!parts.Any())
		{
			return obj?.ToString() ?? "";
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
			throw new Exception("Property '" + propertyName + "' not found.");
		}

		obj = property.GetValue(obj, null);

		return GetValue(parts.Skip(1), obj);
	}

	public override async Task Handle(Globals globals, TemplateSection section)
	{
		if (section.Handler != TemplateHandler.Reflection)
		{
			await Next.Handle(globals, section).ConfigureAwait(false);
			return;
		}

		var parts = section.Content.Trim().Split('.');
		var value = GetValue(parts, globals);

		globals.Write(value);
	}
}