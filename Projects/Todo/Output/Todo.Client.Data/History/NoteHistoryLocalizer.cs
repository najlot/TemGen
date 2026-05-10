using System;
using System.Threading.Tasks;
using Todo.Client.Localisation;

namespace Todo.Client.Data.History;

internal sealed class NoteHistoryLocalizer() : EntityHistoryLocalizerBase
{
	public override bool CanLocalize(string entityName)
		=> string.Equals(entityName, "Note", StringComparison.OrdinalIgnoreCase);

	protected override string[] LocalizePropertyParts(string[] parts)
	{
		if (parts.Length == 1)
		{
			return parts[0] switch
			{
				"Title" => [NoteLoc.Title],
				"Content" => [NoteLoc.Content],
				"Color" => [NoteLoc.Color],
				"IsDeleted" => [CommonLoc.Deleted],
				_ => parts,
			};
		}

		return parts;
	}

	protected override Task<string> LocalizePropertyValue(string[] parts, string value)
	{
		if (parts.Length == 1)
		{
			return Task.FromResult(parts[0] switch
			{
				"Color" => LocalizePredefinedColor(value),
				"IsDeleted" => LocalizeBoolean(value),
				_ => value,
			});
		}

		return Task.FromResult(value);
	}

	private static string LocalizePredefinedColor(string value)
		=> value.ToLowerInvariant() switch
		{
			"white" => PredefinedColorLoc.White,
			"silver" => PredefinedColorLoc.Silver,
			"gray" => PredefinedColorLoc.Gray,
			"black" => PredefinedColorLoc.Black,
			"red" => PredefinedColorLoc.Red,
			"maroon" => PredefinedColorLoc.Maroon,
			"yellow" => PredefinedColorLoc.Yellow,
			"olive" => PredefinedColorLoc.Olive,
			"lime" => PredefinedColorLoc.Lime,
			"green" => PredefinedColorLoc.Green,
			"aqua" => PredefinedColorLoc.Aqua,
			"teal" => PredefinedColorLoc.Teal,
			"blue" => PredefinedColorLoc.Blue,
			"navy" => PredefinedColorLoc.Navy,
			"fuchsia" => PredefinedColorLoc.Fuchsia,
			"purple" => PredefinedColorLoc.Purple,
			_ => value,
		};
}
