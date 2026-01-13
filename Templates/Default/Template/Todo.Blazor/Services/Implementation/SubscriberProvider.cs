using Cosei.Client.Base;
using Cosei.Client.Http;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;

namespace <#cs Write(Project.Namespace)#>.Blazor.Services.Implementation;

public sealed class SubscriberProvider(
	ITokenProvider tokenProvider,
	IHttpClientFactory httpClientFactory,
	ILogger<SubscriberProvider> logger) : ISubscriberProvider, IAsyncDisposable
{
	private SignalRSubscriber? _subscriber;

	public async Task<ISubscriber> GetSubscriber()
	{
		if (_subscriber == null)
		{
			using var client = httpClientFactory.CreateClient();
			var serverUri = (client?.BaseAddress) ?? throw new NullReferenceException("Could not retrieve server connection information!");
			var token = await tokenProvider.GetToken();
			var signalRUri = new Uri(serverUri, "/cosei");

			_subscriber = new SignalRSubscriber(signalRUri.AbsoluteUri,
				options =>
				{
					options.Headers.Add("Authorization", $"Bearer {token}");
				},
				exception =>
				{
					logger.LogError(exception, "Subscriber error.");
				});
		}

		await _subscriber.StartAsync();

		return _subscriber;
	}

	public async Task ClearSubscriber()
	{
		if (_subscriber != null)
		{
			await _subscriber.DisposeAsync();
			_subscriber = null;
		}
	}

	public async ValueTask DisposeAsync()
	{
		await ClearSubscriber();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>