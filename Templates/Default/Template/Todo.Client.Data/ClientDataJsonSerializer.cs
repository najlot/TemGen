using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using <# Project.Namespace#>.Client.Data.Filters;
using <# Project.Namespace#>.Client.Data.GlobalSearch;
using <# Project.Namespace#>.Client.Data.History;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data.Trash;
<#cs
foreach (var featureFolder in Definitions
	.Where(d => !(d.IsEnumeration || d.IsOwnedType))
	.Select(definition => definition.IsArray
		? GetChildFeatureFolderName(definition.Name)
		: FixPathPluralization(definition.Name + "s"))
	.Distinct()
	.OrderBy(featureFolder => featureFolder))
{
	WriteLine($"using {Project.Namespace}.Client.Data.{featureFolder};");
}
#>

namespace <# Project.Namespace#>.Client.Data;

public static class ClientDataJsonSerializer
{
	public static JsonSerializerOptions Options { get; } = CreateOptions();

	private static JsonSerializerOptions CreateOptions()
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

		options.TypeInfoResolverChain.Add(FiltersSerializerContext.Default);
		options.TypeInfoResolverChain.Add(AuthSerializerContext.Default);
		options.TypeInfoResolverChain.Add(GlobalSearchSerializerContext.Default);
		options.TypeInfoResolverChain.Add(HistorySerializerContext.Default);
		options.TypeInfoResolverChain.Add(TrashSerializerContext.Default);
<#cs
foreach (var definition in Definitions.Where(d => !(d.IsEnumeration || d.IsOwnedType)))
{
	WriteLine($"\t\toptions.TypeInfoResolverChain.Add({definition.Name}SerializerContext.Default);");
}
#>		options.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
		return options;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>