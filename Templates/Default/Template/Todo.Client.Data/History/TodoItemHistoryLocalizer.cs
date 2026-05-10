<#cs
string PropertyName(dynamic entry)
{
	return entry.IsReference ? entry.Field + "Id" : entry.Field;
}

string[] Append(string[] source, string value)
{
	var result = new string[source.Length + 1];
	for (var index = 0; index < source.Length; index++)
	{
		result[index] = source[index];
	}

	result[source.Length] = value;
	return result;
}

string BuildPrefixCondition(string[] prefix)
{
	var condition = $"parts.Length == {prefix.Length + 1}";
	for (var index = 0; index < prefix.Length; index++)
	{
		condition += $" && parts[{index}].Equals(\"{prefix[index]}\", StringComparison.OrdinalIgnoreCase)";
	}

	return condition;
}

string BuildArrayLiteral(string[] values)
{
	return "[" + string.Join(", ", values) + "]";
}

dynamic? FindDefinition(string name)
{
	foreach (var definition in Definitions)
	{
		if (definition.Name == name)
		{
			return definition;
		}
	}

	return null;
}

void CollectReferenceEntries(dynamic currentDefinition, System.Collections.Generic.List<dynamic> entries, System.Collections.Generic.HashSet<string> knownTypes)
{
	foreach (var entry in currentDefinition.Entries)
	{
		if (entry.IsReference && knownTypes.Add((string)entry.ReferenceType))
		{
			entries.Add(entry);
		}

		if (entry.IsOwnedType || entry.IsArray)
		{
			var childDefinition = FindDefinition((string)entry.EntryType);
			if (childDefinition is not null)
			{
				CollectReferenceEntries(childDefinition, entries, knownTypes);
			}
		}
	}
}

void CollectEnumTypes(dynamic currentDefinition, System.Collections.Generic.List<string> enumTypes, System.Collections.Generic.HashSet<string> knownTypes)
{
	foreach (var entry in currentDefinition.Entries)
	{
		if (entry.IsEnumeration && knownTypes.Add((string)entry.EntryType))
		{
			enumTypes.Add((string)entry.EntryType);
		}

		if (entry.IsOwnedType || entry.IsArray)
		{
			var childDefinition = FindDefinition((string)entry.EntryType);
			if (childDefinition is not null)
			{
				CollectEnumTypes(childDefinition, enumTypes, knownTypes);
			}
		}
	}
}

dynamic? GetReferenceLabelEntry(string referenceType)
{
	var referenceDefinition = FindDefinition(referenceType);
	if (referenceDefinition is null)
	{
		return null;
	}

	foreach (var entry in referenceDefinition.Entries)
	{
		if (!(entry.IsOwnedType || entry.IsKey || entry.IsArray || entry.IsReference || entry.IsEnumeration))
		{
			return entry;
		}
	}

	return null;
}

void WritePropertyPartsBlocks(dynamic currentDefinition, string[] prefixKeys, string[] prefixDisplays, bool isRoot, ref bool firstBlock)
{
	var hasCases = isRoot;
	foreach (var _ in currentDefinition.Entries)
	{
		hasCases = true;
		break;
	}

	if (hasCases)
	{
		WriteLine(firstBlock
			? $"\t\tif ({BuildPrefixCondition(prefixKeys)})"
			: $"\t\telse if ({BuildPrefixCondition(prefixKeys)})");
		WriteLine("\t\t{");
		WriteLine($"\t\t\treturn parts[{prefixKeys.Length}] switch");
		WriteLine("\t\t\t{");
		foreach (var entry in currentDefinition.Entries)
		{
			var displayPath = Append(prefixDisplays, currentDefinition.Name + "Loc." + entry.Field);
			WriteLine($"\t\t\t\t\"{PropertyName(entry)}\" => {BuildArrayLiteral(displayPath)},");
		}

		if (isRoot)
		{
			WriteLine("\t\t\t\t\"IsDeleted\" => [CommonLoc.Deleted],");
		}

		WriteLine("\t\t\t\t_ => parts,");
		WriteLine("\t\t\t};");
		WriteLine("\t\t}");
		firstBlock = false;
	}

	foreach (var entry in currentDefinition.Entries)
	{
		if (!(entry.IsOwnedType || entry.IsArray))
		{
			continue;
		}

		var childDefinition = FindDefinition((string)entry.EntryType);
		if (childDefinition is null)
		{
			continue;
		}

		WritePropertyPartsBlocks(
			childDefinition,
			Append(prefixKeys, PropertyName(entry)),
			Append(prefixDisplays, currentDefinition.Name + "Loc." + entry.Field),
			false,
			ref firstBlock);
	}
}

string? GetValueLocalizationExpression(dynamic entry)
{
	if (entry.IsReference)
	{
		return $"await Localize{entry.ReferenceType}Id(value).ConfigureAwait(false)";
	}

	if (entry.IsEnumeration)
	{
		return $"Localize{entry.EntryType}(value)";
	}

	if (string.Equals((string)entry.EntryType, "bool", StringComparison.OrdinalIgnoreCase))
	{
		return "LocalizeBoolean(value)";
	}

	return null;
}

void WritePropertyValueBlocks(dynamic currentDefinition, string[] prefixKeys, bool isRoot, ref bool firstBlock)
{
	var hasCases = isRoot;
	foreach (var entry in currentDefinition.Entries)
	{
		if (GetValueLocalizationExpression(entry) is not null)
		{
			hasCases = true;
			break;
		}
	}

	if (hasCases)
	{
		var usesAsync = RequiresAsyncValueLocalization();
		var returnPrefix = usesAsync ? "\t\t\treturn " : "\t\t\treturn Task.FromResult(";
		var returnSuffix = usesAsync ? "\t\t\t};" : "\t\t\t});";

		WriteLine(firstBlock
			? $"\t\tif ({BuildPrefixCondition(prefixKeys)})"
			: $"\t\telse if ({BuildPrefixCondition(prefixKeys)})");
		WriteLine("\t\t{");
		WriteLine($"{returnPrefix}parts[{prefixKeys.Length}] switch");
		WriteLine("\t\t\t{");
		foreach (var entry in currentDefinition.Entries)
		{
			var expression = GetValueLocalizationExpression(entry);
			if (expression is null)
			{
				continue;
			}

			WriteLine($"\t\t\t\t\"{PropertyName(entry)}\" => {expression},");
		}

		if (isRoot)
		{
			WriteLine("\t\t\t\t\"IsDeleted\" => LocalizeBoolean(value),");
		}

		WriteLine("\t\t\t\t_ => value,");
		WriteLine(returnSuffix);
		WriteLine("\t\t}");
		firstBlock = false;
	}

	foreach (var entry in currentDefinition.Entries)
	{
		if (!(entry.IsOwnedType || entry.IsArray))
		{
			continue;
		}

		var childDefinition = FindDefinition((string)entry.EntryType);
		if (childDefinition is null)
		{
			continue;
		}

		WritePropertyValueBlocks(childDefinition, Append(prefixKeys, PropertyName(entry)), false, ref firstBlock);
	}
}

bool RequiresAsyncValueLocalization()
{
	var referenceEntries = new System.Collections.Generic.List<dynamic>();
	CollectReferenceEntries(Definition, referenceEntries, new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase));
	return referenceEntries.Count > 0;
}

var referenceEntries = new System.Collections.Generic.List<dynamic>();
CollectReferenceEntries(Definition, referenceEntries, new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase));

var enumTypes = new System.Collections.Generic.List<string>();
CollectEnumTypes(Definition, enumTypes, new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase));

WriteLine("using System;");
WriteLine("using System.Threading.Tasks;");
foreach (var entry in referenceEntries)
{
	WriteLine($"using {Project.Namespace}.Client.Data.{entry.ReferenceType}s;");
}
WriteLine($"using {Project.Namespace}.Client.Localisation;");
WriteLine(string.Empty);
WriteLine($"namespace {Project.Namespace}.Client.Data.History;");
WriteLine(string.Empty);

if (referenceEntries.Count == 0)
{
	WriteLine($"internal sealed class {Definition.Name}HistoryLocalizer() : EntityHistoryLocalizerBase");
}
else
{
	WriteLine($"internal sealed class {Definition.Name}HistoryLocalizer(");
	for (var index = 0; index < referenceEntries.Count; index++)
	{
		var entry = referenceEntries[index];
		var suffix = index == referenceEntries.Count - 1 ? ") : EntityHistoryLocalizerBase" : ",";
		WriteLine($"\tI{entry.ReferenceType}Service {entry.ReferenceTypeLow}Service{suffix}");
	}
}

WriteLine("{");
WriteLine("\tpublic override bool CanLocalize(string entityName)");
WriteLine($"\t\t=> string.Equals(entityName, \"{Definition.Name}\", StringComparison.OrdinalIgnoreCase);");
WriteLine(string.Empty);
WriteLine("\tprotected override string[] LocalizePropertyParts(string[] parts)");
WriteLine("\t{");
var firstPartBlock = true;
WritePropertyPartsBlocks(Definition, [], [], true, ref firstPartBlock);
WriteLine(string.Empty);
WriteLine("\t\treturn parts;");
WriteLine("\t}");
WriteLine(string.Empty);

if (RequiresAsyncValueLocalization())
{
	WriteLine("\tprotected override async Task<string> LocalizePropertyValue(string[] parts, string value)");
	WriteLine("\t{");
	var firstValueBlock = true;
	WritePropertyValueBlocks(Definition, [], true, ref firstValueBlock);
	WriteLine(string.Empty);
	WriteLine("\t\treturn value;");
	WriteLine("\t}");
}
else
{
	WriteLine("\tprotected override Task<string> LocalizePropertyValue(string[] parts, string value)");
	WriteLine("\t{");
	var firstValueBlock = true;
	WritePropertyValueBlocks(Definition, [], true, ref firstValueBlock);
	WriteLine(string.Empty);
	WriteLine("\t\treturn Task.FromResult(value);");
	WriteLine("\t}");
}

foreach (var entry in referenceEntries)
{
	var labelEntry = GetReferenceLabelEntry((string)entry.ReferenceType);
	WriteLine(string.Empty);
	WriteLine($"\tprivate async Task<string> Localize{entry.ReferenceType}Id(string value)");
	WriteLine("\t{");
	WriteLine("\t\tif (!Guid.TryParse(value, out var id))");
	WriteLine("\t\t{");
	WriteLine("\t\t\treturn value;");
	WriteLine("\t\t}");
	WriteLine("\t\telse if (id == Guid.Empty)");
	WriteLine("\t\t{");
	WriteLine("\t\t\treturn string.Empty;");
	WriteLine("\t\t}");
	WriteLine(string.Empty);
	WriteLine("\t\ttry");
	WriteLine("\t\t{");
	WriteLine($"\t\t\tvar {entry.ReferenceTypeLow} = await {entry.ReferenceTypeLow}Service.GetItemAsync(id).ConfigureAwait(false);");
	if (labelEntry is null)
	{
		WriteLine("\t\t\treturn value;");
	}
	else if (string.Equals((string)labelEntry.EntryType, "string", StringComparison.OrdinalIgnoreCase))
	{
		WriteLine($"\t\t\treturn {entry.ReferenceTypeLow}?.{labelEntry.Field} ?? value;");
	}
	else
	{
		WriteLine($"\t\t\treturn Convert.ToString({entry.ReferenceTypeLow}?.{labelEntry.Field}) ?? value;");
	}
	WriteLine("\t\t}");
	WriteLine("\t\tcatch (Exception)");
	WriteLine("\t\t{");
	WriteLine("\t\t\treturn value;");
	WriteLine("\t\t}");
	WriteLine("\t}");
}

foreach (var enumType in enumTypes)
{
	var enumDefinition = FindDefinition(enumType);
	if (enumDefinition is null)
	{
		continue;
	}

	WriteLine(string.Empty);
	WriteLine($"\tprivate static string Localize{enumType}(string value)");
	WriteLine("\t\t=> value.ToLowerInvariant() switch");
	WriteLine("\t\t{");
	foreach (var enumEntry in enumDefinition.Entries)
	{
		WriteLine($"\t\t\t\"{enumEntry.Field.ToLowerInvariant()}\" => {enumType}Loc.{enumEntry.Field},");
	}
	WriteLine("\t\t\t_ => value,");
	WriteLine("\t\t};");
}

WriteLine("}");
SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray);
#>