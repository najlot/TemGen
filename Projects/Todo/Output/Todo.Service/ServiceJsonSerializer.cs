using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Todo.Service.Features.Auth;
using Todo.Service.Features.Filters;
using Todo.Service.Features.GlobalSearch;
using Todo.Service.Features.History;
using Todo.Service.Features.Trash;
using Todo.Service.Shared.Configuration;
using Todo.Service.Features.Notes;
using Todo.Service.Features.TodoItems;
using Todo.Service.Features.Users;


namespace Todo.Service;

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
		ChecklistTaskSerializerContext.Default,
		NoteSerializerContext.Default,
		TodoItemSerializerContext.Default,
		UserSerializerContext.Default,
	];

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
