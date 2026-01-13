using System.Threading.Tasks;
using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Client.Data.Services.Implementation;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.ClientBase.Models;
using <#cs Write(Project.Namespace)#>.ClientBase.Services.Implementation;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ProfileHandler;

public sealed class LocalProfileHandler : AbstractProfileHandler
{
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly IMap _map;

	public LocalProfileHandler(IMessenger messenger, IDispatcherHelper dispatcher, IMap map)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_map = map;
	}

	protected override async Task ApplyProfile(ProfileBase profile)
	{
		if (profile is LocalProfile localProfile)
		{
			var subscriber = new LocalSubscriber();

			var userStore = new LocalUserStore(localProfile.FolderName, subscriber);
			UserService = new UserService(userStore);
			UserMessagingService = new UserMessagingService(_messenger, _dispatcher, subscriber);
<#cs
var definitions = Definitions.Where(d => !d.IsOwnedType 
	&& !d.IsArray 
	&& !d.IsEnumeration 
	&& !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase));
foreach(var definition in definitions)
{
	WriteLine($"			var {definition.NameLow}Store = new Local{definition.Name}Store(localProfile.FolderName, subscriber, _map);");
	WriteLine($"			{definition.Name}Service = new {definition.Name}Service({definition.NameLow}Store);");
	WriteLine($"			{definition.Name}MessagingService = new {definition.Name}MessagingService(_messenger, _dispatcher, subscriber);");
}

Result = Result.TrimEnd();
#>

			await subscriber.StartAsync();

			Subscriber = subscriber;
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>