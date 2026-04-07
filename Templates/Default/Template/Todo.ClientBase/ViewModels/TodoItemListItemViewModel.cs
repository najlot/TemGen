using System;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.ViewModels;

public class <# Definition.Name#>ListItemViewModel : AbstractViewModel
{
	public Guid Id { get; set => Set(ref field, value); }

<#for entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2)
#>	public <#cs Write(entry.IsArray ? "List<" : "")#><# entry.EntryType#><#cs Write(entry.IsArray ? ">" : entry.IsNullable ? "?" : "")#> <# entry.Field#><#cs Write(entry.IsReference ? "Id" : "")#> { get; set => Set(ref field, value); }<#if entry.EntryType == "string"
#> = string.Empty;<#end#>
<#end
#>}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>
