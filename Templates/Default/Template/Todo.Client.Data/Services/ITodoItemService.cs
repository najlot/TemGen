using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts.Filters;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Services;

public interface I<#cs Write(Definition.Name)#>Service : IDisposable
{
	<#cs Write(Definition.Name)#>Model Create<#cs Write(Definition.Name)#>();
	Task AddItemAsync(<#cs Write(Definition.Name)#>Model item);
	Task<IEnumerable<<#cs Write(Definition.Name)#>ListItemModel>> GetItemsAsync();
	Task<IEnumerable<<#cs Write(Definition.Name)#>ListItemModel>> GetItemsAsync(<#cs Write(Definition.Name)#>Filter filter);
	Task<<#cs Write(Definition.Name)#>Model> GetItemAsync(Guid id);
	Task UpdateItemAsync(<#cs Write(Definition.Name)#>Model item);
	Task DeleteItemAsync(Guid id);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>