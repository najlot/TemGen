using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.Filters;

public interface IFilterRepository : IEntityRepository<FilterModel>
{
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>