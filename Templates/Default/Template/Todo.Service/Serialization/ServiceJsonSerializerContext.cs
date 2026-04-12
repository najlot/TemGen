using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using <# Project.Namespace#>.Contracts.Auth;
using <# Project.Namespace#>.Contracts.GlobalSearch;
using <# Project.Namespace#>.Contracts.History;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Shared.Configuration;
<#cs
foreach (var namespaceName in Definitions
	.Where(d => !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
	.Select(definition =>
	{
		if (definition.IsEnumeration || definition.IsOwnedType || definition.IsArray)
		{
			return $"{Project.Namespace}.Contracts.{GetChildFeatureFolderName(definition.Name)}";
		}

		return $"{Project.Namespace}.Contracts.{definition.Name}s";
	})
	.Concat(Definitions
		.Where(d => !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)
			&& !(d.IsEnumeration || d.IsOwnedType || d.IsArray))
		.Select(definition => $"{Project.Namespace}.Service.Features.{definition.Name}s"))
	.Distinct()
	.OrderBy(namespaceName => namespaceName))
{
	WriteLine($"using {namespaceName};");
}
#>using <# Project.Namespace#>.Contracts.Users;
using <# Project.Namespace#>.Service.Features.Users;

namespace <# Project.Namespace#>.Service.Serialization;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(AuthRequest))]
[JsonSerializable(typeof(GlobalSearchItem[]))]
[JsonSerializable(typeof(List<GlobalSearchItem>))]
[JsonSerializable(typeof(HistoryChange[]))]
[JsonSerializable(typeof(HistoryEntry[]))]
[JsonSerializable(typeof(TrashItem[]))]
[JsonSerializable(typeof(List<TrashItem>))]
[JsonSerializable(typeof(TrashItemCreated))]
[JsonSerializable(typeof(TrashItemUpdated))]
[JsonSerializable(typeof(TrashItemDeleted))]
[JsonSerializable(typeof(HistoryModel))]
[JsonSerializable(typeof(StorageConfiguration))]
[JsonSerializable(typeof(BackupConfiguration))]
[JsonSerializable(typeof(FileConfiguration))]
[JsonSerializable(typeof(LiteDbConfiguration))]
[JsonSerializable(typeof(MongoDbConfiguration))]
[JsonSerializable(typeof(MySqlConfiguration))]
[JsonSerializable(typeof(ServiceConfiguration))]
<#for definition in Definitions.Where(d => !d.IsEnumeration)
#>[JsonSerializable(typeof(<# definition.Name#>))]
<#end#><#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType))
#>[JsonSerializable(typeof(List<<# definition.Name#>ListItem>))]
[JsonSerializable(typeof(<# definition.Name#>ListItem[]))]
[JsonSerializable(typeof(Create<# definition.Name#>))]
[JsonSerializable(typeof(Update<# definition.Name#>))]
[JsonSerializable(typeof(<# definition.Name#>Model))]
[JsonSerializable(typeof(<# definition.Name#>Created))]
[JsonSerializable(typeof(<# definition.Name#>Updated))]
[JsonSerializable(typeof(<# definition.Name#>Deleted))]
<#end#><#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>[JsonSerializable(typeof(<# definition.Name#>Filter))]
<#end#>public partial class ServiceJsonSerializerContext : JsonSerializerContext
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>