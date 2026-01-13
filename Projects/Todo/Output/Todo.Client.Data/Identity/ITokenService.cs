using System.Threading.Tasks;

namespace Todo.Client.Data.Identity;

public interface ITokenService
{
	Task<string> CreateToken(string username, string password);
}