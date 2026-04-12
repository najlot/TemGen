using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Todo.Service.Serialization;

public static class ServiceJsonSerializer
{
	public static JsonSerializerOptions Options { get; } = CreateOptions();

	public static JsonSerializerOptions IndentedOptions { get; } = CreateOptions(writeIndented: true);

	private static JsonSerializerOptions CreateOptions(bool writeIndented = false)
	{
		var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			WriteIndented = writeIndented,
			PropertyNameCaseInsensitive = true,
		};

		options.TypeInfoResolverChain.Add(ServiceJsonSerializerContext.Default);
		options.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
		return options;
	}
}
