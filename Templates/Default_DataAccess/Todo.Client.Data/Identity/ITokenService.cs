using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data.Identity;

public interface ITokenService
{
	Task<string> CreateToken(string username, string password);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>