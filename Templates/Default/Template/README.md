# <# Project.Namespace#> Application
<#for definition in Definitions
    .Where(d => !d.IsArray && !d.IsOwnedType && !d.IsEnumeration && !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))#>
## <# definition.Name#>
<# definition.Name#> has following children:
<#for entry in definition.Entries
#><#if entry.IsArray
#> - <# entry.Field#> (list of <# entry.EntryType#>)
<#elseif entry.IsOwnedType#> - <# entry.Field#> (<# entry.EntryType#>)
<#elseif entry.IsEnumeration#> - <# entry.Field#> (<# entry.EntryType#> enum)
<#end#><#end#><#end#>
<#cs SetOutputPathAndSkipOtherDefinitions()#>