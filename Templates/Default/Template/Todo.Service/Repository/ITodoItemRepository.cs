using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

public interface I<#cs Write(Definition.Name)#>Repository
{
	IAsyncEnumerable<<#cs Write(Definition.Name)#>Model> GetAll();

	IQueryable<<#cs Write(Definition.Name)#>Model> GetAllQueryable();

	Task<<#cs Write(Definition.Name)#>Model?> Get(Guid id);

	Task Insert(<#cs Write(Definition.Name)#>Model model);

	Task Update(<#cs Write(Definition.Name)#>Model model);

	Task Delete(Guid id);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>