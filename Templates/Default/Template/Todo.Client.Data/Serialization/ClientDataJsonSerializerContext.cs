using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.Auth;
using <# Project.Namespace#>.Contracts.GlobalSearch;
using <# Project.Namespace#>.Contracts.History;
using <# Project.Namespace#>.Contracts.Trash;
<#cs
foreach (var contractNamespace in Definitions
	.Where(d => !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
	.Select(definition => 
	{
		if (definition.IsEnumeration || definition.IsOwnedType || definition.IsArray)
		{
			return $"{Project.Namespace}.Contracts.{GetChildFeatureFolderName(definition.Name)}";
		}
		
		return $"{Project.Namespace}.Contracts.{definition.Name}s";
	})
	.Distinct()
	.OrderBy(contractNamespace => contractNamespace))
{
	WriteLine($"using {contractNamespace};");
}
#>using <# Project.Namespace#>.Contracts.Users;

namespace <# Project.Namespace#>.Client.Data.Serialization;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthRequest))]
[JsonSerializable(typeof(GlobalSearchItem[]))]
[JsonSerializable(typeof(HistoryEntry[]))]
[JsonSerializable(typeof(TrashItem[]))]
<#for definition in Definitions.Where(d => !d.IsEnumeration)
#>[JsonSerializable(typeof(<# definition.Name#>))]
<#end#><#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType))
#>[JsonSerializable(typeof(<# definition.Name#>ListItem[]))]
[JsonSerializable(typeof(Create<# definition.Name#>))]
[JsonSerializable(typeof(<# definition.Name#>Created))]
[JsonSerializable(typeof(Update<# definition.Name#>))]
[JsonSerializable(typeof(<# definition.Name#>Updated))]
[JsonSerializable(typeof(<# definition.Name#>Deleted))]
<#end#><#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>[JsonSerializable(typeof(<# definition.Name#>Filter))]
<#end#>public partial class ClientDataJsonSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>