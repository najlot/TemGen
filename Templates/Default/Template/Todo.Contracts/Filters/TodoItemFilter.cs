using System;
using System.Collections.Generic;

namespace <#cs Write(Project.Namespace)#>.Contracts.Filters;

public sealed class <#cs Write(Definition.Name);#>Filter
{
<#cs
foreach(var entry in Entries)
{
    if (entry.IsReference)
    {
        WriteLine($"		public Guid? {entry.Field}Id {{ get; set; }}");
    }
    else if (entry.EntryType == "long"
        || entry.EntryType == "short"
        || entry.EntryType == "int"
        || entry.EntryType == "ulong"
        || entry.EntryType == "ushort"
        || entry.EntryType == "uint"
        || entry.EntryType == "DateTime"
        )
    {
        WriteLine($"	public {entry.EntryType}? {entry.Field}From {{ get; set; }}");
        WriteLine($"	public {entry.EntryType}? {entry.Field}To {{ get; set; }}");
    }
    else if (!(entry.IsArray || entry.IsOwnedType))
    {
        WriteLine($"	public {entry.EntryType}? {entry.Field} {{ get; set; }}");
    }
}

Result = Result.TrimEnd();
#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>