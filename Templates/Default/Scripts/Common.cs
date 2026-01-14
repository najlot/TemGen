void WriteContractParameter()
{
    foreach(var entry in Entries)
    {
        var typePrefix = entry.IsArray ? "List<" : "";
        var typeSuffix = entry.IsNullable ? "?" : "";
        typeSuffix = entry.IsArray ? ">" : typeSuffix;
        var suffix = entry.IsReference? "Id" : "";
        
        WriteLine($"	{typePrefix}{entry.EntryType}{typeSuffix} {entry.FieldLow}{suffix},");
    }

    Result = Result.TrimEnd('\r', '\n', ',');
}

void WriteContractProperties()
{
    foreach(var entry in Entries)
    {
        var typePrefix = entry.IsArray ? "List<" : "";
        var typeSuffix = entry.IsNullable ? "?" : "";
        typeSuffix = entry.IsArray ? ">" : typeSuffix;
        var suffix = entry.IsReference? "Id" : "";
        
        WriteLine($"	public {typePrefix}{entry.EntryType}{typeSuffix} {entry.Field}{suffix} {{ get; }} = {entry.FieldLow}{suffix};");
    }

    Result = Result.TrimEnd();
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
        RelativePath = RelativePath.Replace("Entrys", "Entries").Replace("Statuss", "Status");
    }
}

void SetOutputPathAndSkipOtherDefinitions()
{
    RelativePath = RelativePath.Replace("TodoItem", Definition.Name).Replace("Todo", Project.Namespace);
    RelativePath = RelativePath.Replace("Entrys", "Entries").Replace("Statuss", "Status");
    SkipOtherDefinitions = true;
}

enum MapArrayStrategy
{
    Remap,
    RemapToCustomCollection,
    LeaveAsIs,
    MapInto
}

void WriteFromToMapping(
    string fromSuffix = "",
    string toSuffix = "",
    int max = int.MaxValue,
    MapArrayStrategy arrayStrategy = MapArrayStrategy.Remap,
    bool onlySimple = false)
{
    int count = 0;

    foreach(var entry in Entries)
    {
        if (onlySimple && (entry.IsArray || entry.IsOwnedType || entry.IsReference))
        {
            continue;
        }

        if (count++ >= max)
        {
            break;
        }

        if (entry.IsArray)
        {
            switch(arrayStrategy)
            {
                case MapArrayStrategy.Remap:
                    WriteLine($"		to.{entry.Field} = map.From<{entry.EntryType}{fromSuffix}>(from.{entry.Field}).ToList<{entry.EntryType}{toSuffix}>();");
                    break;
                case MapArrayStrategy.RemapToCustomCollection:
                    WriteLine($"		to.{entry.Field} = [.. map.From<{entry.EntryType}{fromSuffix}>(from.{entry.Field}).To<{entry.EntryType}{toSuffix}>()];");
                    break;
                case MapArrayStrategy.MapInto:
                    WriteLine($"		to.{entry.Field} = map.From<{entry.EntryType}{fromSuffix}>(from.{entry.Field}).ToList(to.{entry.Field});");
                    break;
                default:
                     WriteLine($"		to.{entry.Field} = from.{entry.Field};");
                     break;
            }
        }
        else if (entry.IsOwnedType)
        {
            WriteLine($"		to.{entry.Field} = map.From(from.{entry.Field}).To<{entry.EntryType}{toSuffix}>();");
        }
        else if (entry.IsReference)
        {
            WriteLine($"		to.{entry.Field}Id = from.{entry.Field}Id;");
        }
        else
        {
            WriteLine($"		to.{entry.Field} = from.{entry.Field};");
        }
    }
}

void WriteCtorMapping(
    string fromSuffix = "",
    string toSuffix = "",
    MapArrayStrategy arrayStrategy = MapArrayStrategy.Remap)
{
    string tabs = "\t\t\t";

    foreach(var entry in Entries)
    {
        if (entry.IsArray)
        {
            switch(arrayStrategy)
            {
                case MapArrayStrategy.Remap:
                    WriteLine($"{tabs}map.From<{entry.EntryType}{fromSuffix}>(from.{entry.Field}).ToList<{entry.EntryType}{toSuffix}>(),");
                    break;
                case MapArrayStrategy.RemapToCustomCollection:
                    WriteLine($"{tabs}[.. map.From<{entry.EntryType}{fromSuffix}>(from.{entry.Field}).ToList<{entry.EntryType}{toSuffix}>()],");
                    break;
                default:
                     WriteLine($"{tabs}from.{entry.Field},");
                     break;
            }
        }
        else if (entry.IsOwnedType)
        {
            WriteLine($"{tabs}map.From(from.{entry.Field}).To<{entry.EntryType}{toSuffix}>(),");
        }
        else if (entry.IsReference)
        {
            WriteLine($"{tabs}from.{entry.Field}Id,");
        }
        else
        {
            WriteLine($"{tabs}from.{entry.Field},");
        }
    }

    Result = Result.TrimEnd('\r', '\n', ',');
}