using Najlot.Map;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Repositories.Implementation;

public sealed class UserRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), IUserRepository
{
	public async Task<UserListItemModel[]> GetItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var users = await client.GetFromJsonAsync<UserListItem[]>("api/User").ConfigureAwait(false) ?? [];
		return map.From<UserListItem>(users).ToArray<UserListItemModel>();
	}

	public async Task<UserModel> GetItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var user = await client.GetFromJsonAsync<User>($"api/User/{id}").ConfigureAwait(false);
		return map.From(user).To<UserModel>();
	}

	public async Task<UserModel> GetCurrentUserAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var user = await client.GetFromJsonAsync<User>("api/User/Current").ConfigureAwait(false);
		return map.From(user).To<UserModel>();
	}

	public async Task AddItemAsync(UserModel item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<CreateUser>();
		var response = await client.PostAsJsonAsync("api/User", request).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateItemAsync(UserModel item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<UpdateUser>();
		var response = await client.PutAsJsonAsync("api/User", request).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/User/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>