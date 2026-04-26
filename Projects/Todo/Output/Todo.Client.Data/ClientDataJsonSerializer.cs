using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Todo.Client.Data.Filters;
using Todo.Client.Data.GlobalSearch;
using Todo.Client.Data.History;
using Todo.Client.Data.Identity;
using Todo.Client.Data.Trash;
using Todo.Client.Data.Notes;
using Todo.Client.Data.TodoItems;
using Todo.Client.Data.Users;


namespace Todo.Client.Data;

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
		options.TypeInfoResolverChain.Add(ChecklistTaskSerializerContext.Default);
		options.TypeInfoResolverChain.Add(NoteSerializerContext.Default);
		options.TypeInfoResolverChain.Add(TodoItemSerializerContext.Default);
		options.TypeInfoResolverChain.Add(UserSerializerContext.Default);
		options.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
		return options;
	}
}
