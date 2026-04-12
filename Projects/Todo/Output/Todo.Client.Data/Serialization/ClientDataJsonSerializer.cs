using System.Text.Json;

namespace Todo.Client.Data.Serialization;

public static class ClientDataJsonSerializer
{
	public static JsonSerializerOptions Options { get; } = new(ClientDataJsonSerializerContext.Default.Options);
}
