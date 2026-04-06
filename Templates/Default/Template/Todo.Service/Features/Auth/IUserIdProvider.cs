namespace <# Project.Namespace#>.Service.Features.Auth;

public interface IUserIdProvider
{
	Guid GetRequiredUserId();
	string GetRequiredUsername();
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>