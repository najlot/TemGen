using <#cs Write(Project.Namespace)#>.ClientBase.Models;
using System.Threading.Tasks;
using Cosei.Client.Base;
using <#cs Write(Project.Namespace)#>.ClientBase.Services.Implementation;
using <#cs Write(Project.Namespace)#>.Client.Data.Services;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ProfileHandler;

public abstract class AbstractProfileHandler : IProfileHandler
{
	private IProfileHandler _handler = null;

	protected ISubscriber Subscriber { get; set; }

<#cs
var definitions = Definitions.Where(d => !d.IsOwnedType
	&& !d.IsArray
	&& !d.IsEnumeration
	&& !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase));
foreach(var definition in definitions)
{
	WriteLine($"	protected I{definition.Name}Service {definition.Name}Service {{ get; set; }}");
	WriteLine($"	protected {definition.Name}MessagingService {definition.Name}MessagingService {{ get; set; }}");
}
#>
	protected IUserService UserService { get; set; }
	protected UserMessagingService UserMessagingService { get; set; }
<#cs
var definitions = Definitions.Where(d => !d.IsOwnedType
&& !d.IsArray
&& !d.IsEnumeration
&& !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase));
foreach(var definition in definitions)
{
	WriteLine($"	public I{definition.Name}Service Get{definition.Name}Service() => {definition.Name}Service ?? _handler?.Get{definition.Name}Service();");
}
#>
	public IUserService GetUserService() => UserService ?? _handler?.GetUserService();
	public IProfileHandler SetNext(IProfileHandler handler) => _handler = handler;

	public async Task SetProfile(ProfileBase profile)
	{
		if (Subscriber != null)
		{
			await Subscriber.DisposeAsync();
			Subscriber = null;
		}

<#cs
var definitions = Definitions.Where(d => !d.IsOwnedType 
	&& !d.IsArray 
	&& !d.IsEnumeration 
	&& !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase));
foreach(var definition in definitions)
{
	WriteLine($"		{definition.Name}Service?.Dispose();");
	WriteLine($"		{definition.Name}Service = null;");
	WriteLine($"		{definition.Name}MessagingService?.Dispose();");
	WriteLine($"		{definition.Name}MessagingService = null;");
}
#>
		UserService?.Dispose();
		UserService = null;
		UserMessagingService?.Dispose();
		UserMessagingService = null;

		await ApplyProfile(profile);

		if (_handler != null)
		{
			await _handler.SetProfile(profile);
		}
	}

	protected abstract Task ApplyProfile(ProfileBase profile);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>