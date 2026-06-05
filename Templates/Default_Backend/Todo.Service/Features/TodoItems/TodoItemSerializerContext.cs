using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
<#cs
var featureFolder = Definition.IsArray || Definition.IsOwnedType
	? GetChildFeatureFolderName(Definition.Name)
	: FixPathPluralization(Definition.Name + "s");
var contractNamespace = Definition.IsArray || Definition.IsOwnedType
	? $"{Project.Namespace}.Contracts.{GetChildFeatureFolderName(Definition.Name)}"
	: $"{Project.Namespace}.Contracts.{FixPathPluralization(Definition.Name + "s")}";
WriteLine($"using {contractNamespace};");
#>

namespace <# Project.Namespace#>.Service.Features.<#cs Write(Definition.IsArray || Definition.IsOwnedType ? GetChildFeatureFolderName(Definition.Name) : FixPathPluralization(Definition.Name + "s")); #>;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(<# Definition.Name#>))]
<#if !(Definition.IsArray || Definition.IsEnumeration || Definition.IsOwnedType)
#>[JsonSerializable(typeof(List<<# Definition.Name#>ListItem>))]
[JsonSerializable(typeof(<# Definition.Name#>ListItem[]))]
[JsonSerializable(typeof(Create<# Definition.Name#>))]
[JsonSerializable(typeof(Update<# Definition.Name#>))]
[JsonSerializable(typeof(<# Definition.Name#>Model))]
[JsonSerializable(typeof(<# Definition.Name#>Created))]
[JsonSerializable(typeof(<# Definition.Name#>Updated))]
[JsonSerializable(typeof(<# Definition.Name#>Deleted))]
<#end#><#if !(Definition.IsArray || Definition.IsEnumeration || Definition.IsOwnedType || Definition.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
#><#end
#>public partial class <# Definition.Name#>SerializerContext : JsonSerializerContext
{
}
<#cs
if (Definition.IsEnumeration)
{
	RelativePath = "";
}
else
{
	RelativePath = RelativePath.Replace("TodoItem", Definition.Name).Replace("Todo", Project.Namespace);
	RelativePath = FixPathPluralization(RelativePath);

	if (Definition.IsArray || Definition.IsOwnedType)
	{
		MoveChildToFeatureFolder();
	}

	SetEncodingWithoutBom();
}
#>