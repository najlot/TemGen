using System;
using System.Collections.Generic;

namespace <# Project.Namespace#>.Contracts;

public <#cs Write(Definition.IsEnumeration ? "enum" : "class")#> <# Definition.Name#>
{
<#if Definition.IsEnumeration
#><#for entry in Entries
#>	<# entry.Field#>,
<#end#><#else
#>	public <#cs Write(Definition.IsArray ? "int" : "Guid")#> Id { get; set; }
<#for entry in Entries
#>	public <#cs Write(entry.IsArray ? "List<" : "")#><# entry.EntryType#><#cs Write(entry.IsArray ? ">" : entry.IsNullable ? "?" : "")#> <# entry.Field#><#cs Write(entry.IsReference ? "Id" : "")#> { get; set; }<#if entry.EntryType == "string"
#> = string.Empty;<#elseif entry.IsArray
#> = [];<#end#>
<#end#><#end#>
}<#cs SetOutputPath(false)#>