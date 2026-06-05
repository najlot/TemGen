using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data.Identity;

public interface IPasswordResetService
{
	Task<PasswordResetResult> RequestPasswordReset(string email);
	Task<PasswordResetResult> ResetPassword(string email, string code, string password);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>