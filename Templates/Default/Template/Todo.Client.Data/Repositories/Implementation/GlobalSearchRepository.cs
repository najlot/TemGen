using Najlot.Map;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Client.Data.Repositories.Implementation;

public sealed class GlobalSearchRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), IGlobalSearchRepository
{
	public async Task<GlobalSearchItemModel[]> SearchAsync(string text)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var encodedText = Uri.EscapeDataString(text);
		var items = await client.GetFromJsonAsync<GlobalSearchItem[]>($"api/GlobalSearch?text={encodedText}").ConfigureAwait(false) ?? [];
		return map.From<GlobalSearchItem>(items).ToArray<GlobalSearchItemModel>();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>