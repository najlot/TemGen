namespace <# Project.Namespace#>.Service.Services;

public class PermissionQueryFilter(IUserIdProvider userIdProvider) : IPermissionQueryFilter
{
	public IQueryable<T> ApplyReadFilter<T>(IQueryable<T> query)
	{
		_ = userIdProvider.GetRequiredUserId();
		return query;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>