namespace <# Project.Namespace#>.Service.Features.Auth;

public interface IPermissionQueryFilter
{
	IQueryable<T> ApplyReadFilter<T>(IQueryable<T> query);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>