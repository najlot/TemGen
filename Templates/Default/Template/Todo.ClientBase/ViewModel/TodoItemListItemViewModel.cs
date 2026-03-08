using System;
using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class <#cs Write(Definition.Name);#>ListItemViewModel : AbstractViewModel
{
	public Guid Id { get; set => Set(ref field, value); }

<#cs
foreach(var entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2))
{
	var typePrefix = entry.IsArray ? "List<" : "";
	var typeSuffix = entry.IsNullable ? "?" : "";
	typeSuffix = entry.IsArray? ">" : typeSuffix;
	var suffix = entry.IsReference ? "Id" : "";
	var def = "";
	if (entry.EntryType == "string")
	{
		def = " = string.Empty;";
	}
	WriteLine($"	public {typePrefix}{entry.EntryType}{typeSuffix} {entry.Field}{suffix} {{ get; set => Set(ref field, value); }}{def}");
}

Result = Result.TrimEnd();
#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>
