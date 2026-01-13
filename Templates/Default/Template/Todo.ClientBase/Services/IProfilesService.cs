using System.Collections.Generic;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.ClientBase.Models;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Services;

public interface IProfilesService
{
	Task<List<ProfileBase>> LoadAsync();
	Task RemoveAsync(ProfileBase profile);
	Task SaveAsync(List<ProfileBase> profiles);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>