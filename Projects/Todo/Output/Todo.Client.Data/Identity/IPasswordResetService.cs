using System.Threading.Tasks;

namespace Todo.Client.Data.Identity;

public interface IPasswordResetService
{
	Task<PasswordResetResult> RequestPasswordReset(string email);
	Task<PasswordResetResult> ResetPassword(string email, string code, string password);
}