using Najlot.Map;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data.Serialization;
using <# Project.Namespace#>.Contracts.Users;

namespace <# Project.Namespace#>.Client.Data.Users;

public sealed class UserRepository(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, IMap map)
	: HttpClientRepository(httpClientFactory, tokenProvider), IUserRepository
{
	public async Task<UserListItemModel[]> GetItemsAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var users = await client.GetFromJsonAsync<UserListItem[]>("api/User", ClientDataJsonSerializer.Options).ConfigureAwait(false) ?? [];
		return map.From<UserListItem>(users).ToArray<UserListItemModel>();
	}

	public async Task<UserModel> GetItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var user = await client.GetFromJsonAsync<User>($"api/User/{id}", ClientDataJsonSerializer.Options).ConfigureAwait(false);
		return map.From(user).To<UserModel>();
	}

	public async Task<UserModel> GetCurrentUserAsync()
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var user = await client.GetFromJsonAsync<User>("api/User/Current", ClientDataJsonSerializer.Options).ConfigureAwait(false);
		return map.From(user).To<UserModel>();
	}

	public async Task AddItemAsync(UserModel item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<CreateUser>();
		var response = await client.PostAsJsonAsync("api/User", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateItemAsync(UserModel item)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var request = map.From(item).To<UpdateUser>();
		var response = await client.PutAsJsonAsync("api/User", request, ClientDataJsonSerializer.Options).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteItemAsync(Guid id)
	{
		using var client = await GetAuthorizedHttpClient().ConfigureAwait(false);
		var response = await client.DeleteAsync($"api/User/{id}").ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>