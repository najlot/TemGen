using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Identity;

public interface ITokenService
{
	Task<string> CreateToken(string username, string password);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>