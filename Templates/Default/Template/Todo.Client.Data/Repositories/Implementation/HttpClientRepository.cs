using System.Net.Http;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Repositories.Implementation;

public abstract class HttpClientRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
{
	protected HttpClient GetAuthorizedHttpClient(string token)
	{
		var client = httpClientFactory.CreateClient();
		client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		return client;
	}

	protected async Task<HttpClient> GetAuthorizedHttpClient()
	{
		var token = await tokenProvider.GetToken().ConfigureAwait(false);
		return GetAuthorizedHttpClient(token);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
