using <#cs Write(Project.Namespace)#>.Client.Data.Services;
using <#cs Write(Project.Namespace)#>.ClientBase.Models;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ProfileHandler;

public interface IProfileHandler
{
	IUserService GetUserService();
<#cs
var definitions = Definitions.Where(d => !d.IsOwnedType 
	&& !d.IsArray 
	&& !d.IsEnumeration 
	&& !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase));
foreach(var definition in definitions)
{
	WriteLine($"	I{definition.Name}Service Get{definition.Name}Service();");
}
#>
	IProfileHandler SetNext(IProfileHandler handler);

	Task SetProfile(ProfileBase profile);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>