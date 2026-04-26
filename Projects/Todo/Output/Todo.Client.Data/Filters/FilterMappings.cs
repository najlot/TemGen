using Najlot.Map.Attributes;
using Todo.Contracts.Filters;

namespace Todo.Client.Data.Filters;

[Mapping]
internal sealed partial class FilterMappings
{
	public static partial void MapToCreate(Filter from, CreateFilter to);

	public static partial void MapToUpdate(Filter from, UpdateFilter to);
}
