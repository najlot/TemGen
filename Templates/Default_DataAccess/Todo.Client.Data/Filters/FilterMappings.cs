using Najlot.Map.Attributes;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.Client.Data.Filters;

[Mapping]
internal sealed partial class FilterMappings
{
	public static partial void MapToCreate(Filter from, CreateFilter to);

	public static partial void MapToUpdate(Filter from, UpdateFilter to);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>