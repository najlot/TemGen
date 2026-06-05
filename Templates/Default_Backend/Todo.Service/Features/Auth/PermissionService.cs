namespace <# Project.Namespace#>.Service.Features.Auth;

public class PermissionService(IUserIdProvider userIdProvider) : IPermissionService
{
	public IQueryable<T> ApplyReadFilter<T>(IQueryable<T> query)
	{
		_ = userIdProvider.GetRequiredUserId();
		return query;
	}

	public bool CanAccess<T>(T item)
	{
		_ = userIdProvider.GetRequiredUserId();
		return true;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>