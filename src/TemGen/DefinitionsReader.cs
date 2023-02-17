using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TemGen;

public static class DefinitionsReader
{
	private static bool IsOwnedDefinition(Definition definition, List<Definition> definitions)
	{
		var result = false;

		foreach (var d in definitions)
		{
			foreach (var e in d.Entries)
			{
				if (!(e.IsEnumeration || e.IsArray || e.IsEnumeration) && e.EntryType == definition.Name)
				{
					e.IsOwnedType = true;
					result = true;
				}
			}
		}

		return result;
	}

	private static bool IsArrayDefinition(Definition definition, List<Definition> definitions)
	{
		foreach (var d in definitions)
		{
			foreach (var e in d.Entries)
			{
				if (e.IsArray && e.EntryType == definition.Name)
				{
					return true;
				}
			}
		}

		return false;
	}

	private static bool IsEnumerationEntry(DefinitionEntry entry, List<Definition> definitions)
	{
		foreach (var d in definitions)
		{
			if (d.IsEnumeration && entry.EntryType == d.Name)
			{
				return true;
			}
		}

		return false;
	}

	private static string GetReferenceEntryType(DefinitionEntry entry, List<Definition> definitions)
	{
		foreach (var d in definitions)
		{
			if (d.Name == entry.ReferenceType)
			{
				foreach (var e in d.Entries)
				{
					if (e.Field == "Id")
					{
						return e.EntryType;
					}
				}
			}
		}

		return "Guid";
	}

	private static string GetLowName(string from) => from[0].ToString().ToLower() + from.Substring(1);

	public static List<Definition> ReadDefinitions(string definitionPath)
	{
		var definitions = new List<Definition>();

		foreach (var path in Directory.GetFiles(definitionPath, "*", SearchOption.TopDirectoryOnly))
		{
			var lines = File.ReadAllLines(path)
				.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith('#')).ToList();

			if (lines.Count == 0)
			{
				continue;
			}

			var name = Path.GetFileNameWithoutExtension(path);

			var definition = new Definition()
			{
				Name = name,
				NameLow = GetLowName(name)
			};

			if (!lines[0].Contains(' '))
			{
				definition.IsEnumeration = true;

				definition.Entries = lines.Select(line =>
				{
					return new DefinitionEntry()
					{
						EntryType = name,
						EntryTypeLow = GetLowName(name),
						Field = line,
						FieldLow = GetLowName(line),
						IsEnumeration = true
					};
				}).ToList();
			}
			else
			{
				definition.Entries = lines.Select(line =>
				{
					var index = line.IndexOf(' ');
					var field = line.Substring(index + 1);
					var entryType = line.Substring(0, index);

					var entry = new DefinitionEntry()
					{
						IsKey = field == "Id",
						EntryType = entryType,
						EntryTypeLow = GetLowName(entryType),
						Field = field,
						FieldLow = GetLowName(field)
					};

					if (entry.EntryType.EndsWith("?"))
					{
						entry.IsNullable = true;
						entry.EntryType = entry.EntryType[0..^1];
						entry.EntryTypeLow = entry.EntryTypeLow[0..^1];
					}
					else if (entry.EntryType.EndsWith("[]"))
					{
						entry.EntryType = entry.EntryType[0..^2];
						entry.EntryTypeLow = entry.EntryTypeLow[0..^2];
						entry.IsArray = true;
					}
					else if (entry.EntryType.EndsWith("Id"))
					{
						entry.ReferenceType = entry.EntryType[0..^2];
						entry.ReferenceTypeLow = GetLowName(entry.ReferenceType);
						entry.IsReference = true;
					}

					return entry;
				}).ToList();
			}

			definitions.Add(definition);
		}

		foreach (var definition in definitions)
		{
			foreach (var e in definition.Entries)
			{
				e.IsEnumeration = IsEnumerationEntry(e, definitions);

				if (e.IsReference)
				{
					e.EntryType = GetReferenceEntryType(e, definitions);
					e.EntryTypeLow = GetLowName(e.EntryType);
				}
			}
		}

		foreach (var definition in definitions)
		{
			definition.IsArray = IsArrayDefinition(definition, definitions);
			definition.IsOwnedType = IsOwnedDefinition(definition, definitions);
		}

		return definitions;
	}
}