using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Localisation;
using Todo.Contracts.History;

namespace Todo.Client.Data.History;

internal abstract class EntityHistoryLocalizerBase : IEntityHistoryLocalizer
{
	public abstract bool CanLocalize(string entityName);
	protected abstract string[] LocalizePropertyParts(string[] parts);
	protected abstract Task<string> LocalizePropertyValue(string[] parts, string value);

	public async Task<HistoryChange> Localize(HistoryChange change)
	{
		if (string.IsNullOrWhiteSpace(change.Path))
		{
			return change;
		}

		var parts = change.Path.Split('.').Select(ParsePropertyName).ToArray();
		var keyLessParts = parts.Select(p => p.PropertyName).ToArray();
		var localizedParts = LocalizePropertyParts(keyLessParts);

		for (var index = 0; index < parts.Length; index++)
		{
			if (parts[index].Key is not null)
			{
				localizedParts[index] += $"[{parts[index].Key}]";
			}
		}

		var localizedPath = string.Join("->", localizedParts);
		var localizedOldValue = await LocalizePropertyValue(keyLessParts, change.OldValue).ConfigureAwait(false);
		var localizedNewValue = await LocalizePropertyValue(keyLessParts, change.NewValue).ConfigureAwait(false);

		return new HistoryChange
		{
			Path = localizedPath,
			OldValue = localizedOldValue,
			NewValue = localizedNewValue
		};
	}

	private static (string PropertyName, string? Key) ParsePropertyName(string part)
	{
		var index = part.IndexOf('[');
		if (index > 0)
		{
			var endIndex = part.IndexOf(']', index);
			if (endIndex > 0)
			{
				var key = part.Substring(index + 1, endIndex - index - 1);
				var propertyName = part.Substring(0, index);
				return (propertyName, key);
			}
		}

		return (part, null);
	}

	protected static string LocalizeBoolean(string value)
		=> string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ? CommonLoc.Yes : CommonLoc.No;
}
