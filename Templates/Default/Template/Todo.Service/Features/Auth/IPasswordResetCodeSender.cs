using System;
using System.Threading.Tasks;

namespace <# Project.Namespace#>.Service.Features.Auth;

public interface IPasswordResetCodeSender
{
	bool CanSend { get; }
	Task SendAsync(string email, string username, string code, TimeSpan expiresIn, string culture);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>