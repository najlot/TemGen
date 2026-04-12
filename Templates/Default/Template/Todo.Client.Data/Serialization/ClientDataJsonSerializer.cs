using System.Text.Json;

namespace <# Project.Namespace#>.Client.Data.Serialization;

public static class ClientDataJsonSerializer
{
	public static JsonSerializerOptions Options { get; } = new(ClientDataJsonSerializerContext.Default.Options);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>