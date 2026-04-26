using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;

public interface I<# Definition.Name#>Repository
{
	Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync();

	Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync(EntityFilter filter);

	Task<<# Definition.Name#>Model> GetItemAsync(Guid id);

	Task AddItemAsync(<# Definition.Name#>Model item);

	Task UpdateItemAsync(<# Definition.Name#>Model item);

	Task DeleteItemAsync(Guid id);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>