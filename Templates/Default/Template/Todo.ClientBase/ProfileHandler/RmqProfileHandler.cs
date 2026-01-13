using Cosei.Client.Base;
using Cosei.Client.RabbitMq;
using Najlot.Map;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.ClientBase.Models;
using <#cs Write(Project.Namespace)#>.ClientBase.Services.Implementation;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;
using <#cs Write(Project.Namespace)#>.Client.Data.Repositories.Implementation;
using <#cs Write(Project.Namespace)#>.Client.Data.Services.Implementation;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ProfileHandler;

public sealed class RmqProfileHandler : AbstractProfileHandler
{
	private RmqProfile _profile;
	private RabbitMqModelFactory _rabbitMqModelFactory;
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly IErrorService _errorService;
	private readonly IMap _map;

	public RmqProfileHandler(IMessenger messenger, IDispatcherHelper dispatcher, IErrorService errorService, IMap map)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_errorService = errorService;
		_map = map;
	}

	private IRequestClient CreateRequestClient()
	{
		return new RabbitMqClient(_rabbitMqModelFactory, "<#cs Write(Project.Namespace)#>.Service");
	}

	protected override async Task ApplyProfile(ProfileBase profile)
	{
		if (_rabbitMqModelFactory != null)
		{
			_rabbitMqModelFactory.Dispose();
			_rabbitMqModelFactory = null;
		}

		if (profile is RmqProfile rmqProfile)
		{
			_profile = rmqProfile;

			_rabbitMqModelFactory = new RabbitMqModelFactory(
				_profile.RabbitMqHost,
				_profile.RabbitMqVirtualHost,
				_profile.RabbitMqUser,
				_profile.RabbitMqPassword);

			var requestClient = CreateRequestClient();
			var tokenProvider = new TokenProvider(CreateRequestClient, _profile.ServerUser, _profile.ServerPassword);
			var subscriber = new RabbitMqSubscriber(
				_rabbitMqModelFactory,
				exception =>
				{
					_dispatcher.BeginInvokeOnMainThread(async () => await _errorService.ShowAlertAsync(exception));
				});

			var userStore = new UserRepository(requestClient, tokenProvider, _map);
			UserService = new UserService(userStore);
			UserMessagingService = new UserMessagingService(_messenger, _dispatcher, subscriber);
<#cs
var definitions = Definitions.Where(d => !d.IsOwnedType 
	&& !d.IsArray 
	&& !d.IsEnumeration 
	&& !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase));
foreach(var definition in definitions)
{
	WriteLine($"			var {definition.NameLow}Store = new {definition.Name}Repository(requestClient, tokenProvider, _map);");
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