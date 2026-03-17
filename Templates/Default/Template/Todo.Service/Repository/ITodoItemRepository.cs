using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository;

public interface I<# Definition.Name#>Repository
{
	IAsyncEnumerable<<# Definition.Name#>Model> GetAll();

	IQueryable<<# Definition.Name#>Model> GetAllQueryable();

	Task<<# Definition.Name#>Model?> Get(Guid id);

	Task Insert(<# Definition.Name#>Model model);

	Task Update(<# Definition.Name#>Model model);

	Task Delete(Guid id);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>