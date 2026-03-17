using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.Client.Data.Repositories;

public interface I<# Definition.Name#>Repository
{
	Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync();

	Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync(<# Definition.Name#>Filter filter);

	Task<<# Definition.Name#>Model> GetItemAsync(Guid id);

	Task AddItemAsync(<# Definition.Name#>Model item);

	Task UpdateItemAsync(<# Definition.Name#>Model item);

	Task DeleteItemAsync(Guid id);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>