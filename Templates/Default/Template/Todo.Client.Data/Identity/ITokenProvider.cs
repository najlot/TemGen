using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Identity;

public interface ITokenProvider
{
	void ClearCache();
	Task<string> GetToken();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>