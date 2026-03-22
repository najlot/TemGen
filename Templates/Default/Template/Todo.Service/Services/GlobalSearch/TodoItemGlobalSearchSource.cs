using Najlot.Map;
using <# Project.Namespace#>.Contracts;
using <# Project.Namespace#>.Service.Repository;

namespace <# Project.Namespace#>.Service.Services.GlobalSearch;

public class <# Definition.Name#>GlobalSearchSource(
	I<# Definition.Name#>Repository <# Definition.NameLow#>Repository,
	IMap map,
	IPermissionQueryFilter permissionQueryFilter) : IGlobalSearchSource
{
	public IAsyncEnumerable<GlobalSearchItem> SearchAsync(string text)
	{
		var normalizedText = text.ToLower();
		var query = permissionQueryFilter
			.ApplyReadFilter(<# Definition.NameLow#>Repository.GetAllQueryable())
			.Where(item => item.DeletedAt == null)
<#cs var searchableEntries = Entries.Where(e => e.EntryType.Equals("string", StringComparison.OrdinalIgnoreCase)).Take(2).ToList();
if (searchableEntries.Count == 0)
{
	WriteLine("\t\t\t.Where(item => false);");
}
else
{
	WriteLine("\t\t\t.Where(item =>");
	for (var i = 0; i < searchableEntries.Count; i++)
	{
		var entry = searchableEntries[i];
		var suffix = i < searchableEntries.Count - 1 ? " ||" : ");";
		WriteLine($"\t\t\t\titem.{entry.Field}.Contains(normalizedText){suffix}");
	}
}
#>
		return map.From(query).To<GlobalSearchItem>().ToAsyncEnumerable();
	}
}<#cs SetOutputPath(Definition.IsOwnedType
	|| Definition.IsEnumeration
	|| Definition.IsArray
	|| Definition.NameLow == "user")#>