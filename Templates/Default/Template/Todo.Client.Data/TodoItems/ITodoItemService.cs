using System;
using System.Threading.Tasks;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;

public interface I<# Definition.Name#>Service
{
	event AsyncEventHandler<<# Definition.Name#>Created>? ItemCreated;
	event AsyncEventHandler<<# Definition.Name#>Updated>? ItemUpdated;
	event AsyncEventHandler<<# Definition.Name#>Deleted>? ItemDeleted;

	Task StartEventListener();

	<# Definition.Name#>Model Create<# Definition.Name#>();
	Task AddItemAsync(<# Definition.Name#>Model item);
	Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync();
	Task<<# Definition.Name#>ListItemModel[]> GetItemsAsync(<# Definition.Name#>Filter filter);
	Task<<# Definition.Name#>Model> GetItemAsync(Guid id);
	Task UpdateItemAsync(<# Definition.Name#>Model item);
	Task DeleteItemAsync(Guid id);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>