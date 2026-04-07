using Najlot.Map;
using Najlot.Map.Attributes;
using System.Text.Json;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Service.Features.History;

[Mapping]
internal partial class HistoryMappings
{
	[MapIgnoreProperty(nameof(HistoryEntry.Changes))]
	[PostMap(nameof(PostMapToModel))]
	public static partial void MapToModel(IMap map, HistoryModel from, HistoryEntry to);
	private static void PostMapToModel(IMap map, HistoryModel from, HistoryEntry to)
	{
		if (string.IsNullOrEmpty(from.Changes))
		{
			to.Changes = [];
		}
		else
		{
			to.Changes = JsonSerializer.Deserialize<HistoryChange[]>(from.Changes) ?? [];
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>