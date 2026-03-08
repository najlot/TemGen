using System.Threading.Tasks;

namespace Todo.Client.Data.Identity;

public interface ITokenProvider
{
	void ClearCache();
	Task<string> GetToken();
}