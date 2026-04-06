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

        SetEncodingWithoutBom();
    }
}

void SetOutputPathAndSkipOtherDefinitions()
{
    RelativePath = RelativePath.Replace("TodoItem", Definition.Name).Replace("Todo", Project.Namespace);
    RelativePath = RelativePath.Replace("Entrys", "Entries").Replace("Statuss", "Status");
    SkipOtherDefinitions = true;
    SetEncodingWithoutBom();
}
