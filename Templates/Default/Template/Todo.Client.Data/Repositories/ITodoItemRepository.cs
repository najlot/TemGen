using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Repositories;

public interface I<#cs Write(Definition.Name)#>Repository : IDisposable
{
	Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync();

	Task<<#cs Write(Definition.Name)#>ListItemModel[]> GetItemsAsync(<#cs Write(Definition.Name)#>Filter filter);

	Task<<#cs Write(Definition.Name)#>Model> GetItemAsync(Guid id);

	Task AddItemAsync(<#cs Write(Definition.Name)#>Model item);

	Task UpdateItemAsync(<#cs Write(Definition.Name)#>Model item);

	Task DeleteItemAsync(Guid id);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>