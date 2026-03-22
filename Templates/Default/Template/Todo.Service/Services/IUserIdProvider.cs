namespace <# Project.Namespace#>.Service.Services;

public interface IUserIdProvider
{
	Guid GetRequiredUserId();
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>