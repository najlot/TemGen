void WriteContractProperties()
{
    foreach(var entry in Entries)
    {
        var typePrefix = entry.IsArray ? "List<" : "";
        var typeSuffix = entry.IsNullable ? "?" : "";
        typeSuffix = entry.IsArray ? ">" : typeSuffix;
        var suffix = entry.IsReference? "Id" : "";
        
        Write($"	public {typePrefix}{entry.EntryType}{typeSuffix} {entry.Field}{suffix}");
        WriteLine($" {{ get; set; }}{GetContractDefaultValue(entry)}");
    }

    Result = Result.TrimEnd('\r', '\n', ',');
}

string GetContractDefaultValue(dynamic? entry)
{
    if (entry == null)
    {
        return string.Empty;
    }

    if (entry.EntryType == "string")
    {
        return " = string.Empty;";
    }

    if (entry.IsArray)
    {
        return " = [];";
    }

    if (entry.IsOwnedType)
    {
        return " = new();";
    }

    return string.Empty;
}

void SetEncodingWithoutBom()
{
    if (RelativePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
        || RelativePath.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)
        || RelativePath.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase)
        || RelativePath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)
        || RelativePath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
        || RelativePath.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase))
    {
        Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}

string FixPathPluralization(string path)
    => path.Replace("Entrys", "Entries").Replace("Statuss", "Status");

bool IsNumberType(string entryType)
    => entryType == "long"
        || entryType == "short"
        || entryType == "int"
        || entryType == "ulong"
        || entryType == "ushort"
        || entryType == "uint"
        || entryType == "decimal"
        || entryType == "double"
        || entryType == "float";

bool IsComparableType(string entryType)
    => IsNumberType(entryType)
        || entryType == "DateTime";

bool AllowsEmptyFilter(bool isNullable, string entryType)
    => isNullable || entryType == "string";

string GetFieldPropertyName(string field, bool isReference)
    => isReference ? field + "Id" : field;

string GetFilterFieldKind(string entryType, bool isReference, bool isEnumeration)
{
    if (isReference || isEnumeration || entryType == "bool")
    {
        return "Option";
    }

    if (entryType == "DateTime")
    {
        return "DateTime";
    }

    if (IsNumberType(entryType))
    {
        return "Number";
    }

    return "Text";
}

void SetOutputPathAndSkipOtherDefinitions()
{
    RelativePath = RelativePath.Replace("TodoItem", Definition.Name).Replace("Todo", Project.Namespace);
    RelativePath = FixPathPluralization(RelativePath);
    SkipOtherDefinitions = true;
    SetEncodingWithoutBom();
}

void SetOutputPath(bool skip)
{
    if (skip)
    {
        RelativePath = "";
    }
    else if (Definition.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
    {
        RelativePath = "";
    }
    else
    {
        RelativePath = RelativePath.Replace("TodoItem", Definition.Name).Replace("Todo", Project.Namespace);
        RelativePath = FixPathPluralization(RelativePath);

        SetEncodingWithoutBom();
    }
}

void MoveChildToFeatureFolder()
{
    var lastIndex = RelativePath.LastIndexOfAny(['\\', '/']);
    if (lastIndex == -1)
    {
        return;
    }
    var preLastIndex = RelativePath.LastIndexOfAny(['\\', '/'], lastIndex - 1);
    var toReplace = RelativePath.Substring(preLastIndex, lastIndex - preLastIndex + 1);
    var replaced = toReplace.Replace(FixPathPluralization(Definition.Name + "s"), GetChildFeatureFolderName(Definition.Name));
    RelativePath = RelativePath.Replace(toReplace, replaced);
}

IEnumerable<string> GetDefinitionUsageNames(string definitionName, int depth = 0)
{
    var usages = Definitions
        .Where(d => d.Name != definitionName)
        .Where(d => d.Entries.Any(entry => entry.EntryType == definitionName));

    foreach (var usage in usages)
    {
        if (usage.IsOwnedType || usage.IsArray)
        {
            foreach (var parentUsage in GetDefinitionUsageNames(usage.Name, depth + 1))
            {
                yield return parentUsage;
            }
        }

        yield return usage.Name;
    }
}

string GetChildFeatureFolderName(string definitionName)
{
    var usages = GetDefinitionUsageNames(definitionName).ToList();
    return usages.Count == 1
        ? FixPathPluralization(usages[0] + "s")
        : "Shared";
}

bool NeedsSharedOwnedChildren() => Entries
    .Where(e => e.IsOwnedType)
    .Where(e => GetDefinitionUsageNames(e.EntryType).Count() > 1)
    .Any();

bool NeedsSharedArrayChildren() => Entries
    .Where(e => e.IsArray)
    .Where(e => GetDefinitionUsageNames(e.EntryType).Count() > 1)
    .Any();

bool NeedsSharedEnumerationChildren() => Entries
    .Where(e => e.IsEnumeration)
    .Where(e => GetDefinitionUsageNames(e.EntryType).Count() > 1)
    .Any();