using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using <# Project.Namespace#>.Service.Features.Auth;
using <# Project.Namespace#>.Service.Features.Filters;
using <# Project.Namespace#>.Service.Features.GlobalSearch;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Features.Trash;
using <# Project.Namespace#>.Service.Shared.Configuration;
<#cs
foreach (var featureFolder in Definitions
	.Where(d => !d.IsEnumeration)
	.Select(definition => definition.IsArray || definition.IsOwnedType
		? GetChildFeatureFolderName(definition.Name)
		: FixPathPluralization(definition.Name + "s"))
	.Distinct()
	.OrderBy(featureFolder => featureFolder))
{
	WriteLine($"using {Project.Namespace}.Service.Features.{featureFolder};");
}
#>

namespace <# Project.Namespace#>.Service;

public static class ServiceJsonSerializer
{
	internal static IJsonTypeInfoResolver[] TypeInfoResolvers { get; } =
	[
		AuthSerializerContext.Default,
		FiltersSerializerContext.Default,
		GlobalSearchSerializerContext.Default,
		HistorySerializerContext.Default,
		TrashSerializerContext.Default,
		ConfigurationSerializerContext.Default,
<#cs
foreach (var definition in Definitions.Where(d => !d.IsEnumeration))
{
	WriteLine($"\t\t{definition.Name}SerializerContext.Default,");
}
#>	];

	public static JsonSerializerOptions Options { get; } = CreateOptions();

	public static JsonSerializerOptions IndentedOptions { get; } = CreateOptions(writeIndented: true);

	private static JsonSerializerOptions CreateOptions(bool writeIndented = false)
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			WriteIndented = writeIndented,
		};

		foreach (var resolver in TypeInfoResolvers)
		{
			options.TypeInfoResolverChain.Add(resolver);
		}

		options.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
		return options;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>