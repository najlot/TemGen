using System;
using System.Collections.Generic;
<#if NeedsSharedEnumerationChildren() || NeedsSharedOwnedChildren() || NeedsSharedArrayChildren()
#>using <# Project.Namespace#>.Contracts.Shared;
<#end#>
namespace <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

public sealed class <# Definition.Name#>Filter
{
<#for entry in Entries
#><#if entry.IsReference
#>	public Guid? <# entry.Field#>Id { get; set; }
<#elseif entry.EntryType == "long"
    || entry.EntryType == "short"
    || entry.EntryType == "int"
    || entry.EntryType == "ulong"
    || entry.EntryType == "ushort"
    || entry.EntryType == "uint"
    || entry.EntryType == "DateTime"
    
#>	public <# entry.EntryType#>? <# entry.Field#>From { get; set; }
	public <# entry.EntryType#>? <# entry.Field#>To { get; set; }
<#elseif !(entry.IsArray || entry.IsOwnedType)
#>	public <# entry.EntryType#>? <# entry.Field#> { get; set; }
<#end#><#end
#>}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>