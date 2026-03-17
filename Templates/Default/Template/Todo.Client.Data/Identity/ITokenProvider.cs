using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.Data.Identity;

public interface ITokenProvider
{
	void ClearCache();
	Task<string> GetToken();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>