using Cosei.Client.Base;
using Najlot.Map;
using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Commands;
using <#cs Write(Project.Namespace)#>.Contracts.ListItems;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Repositories.Implementation;

public sealed class UserRepository : IUserRepository
{
	private readonly IRequestClient _client;
	private readonly ITokenProvider _tokenProvider;
	private readonly IMap _map;

	public UserRepository(IRequestClient client, ITokenProvider tokenProvider, IMap map)
	{
		_tokenProvider = tokenProvider;
		_client = client;
		_map = map;
	}

	public async Task<UserListItemModel[]> GetItemsAsync()
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var users = await _client.GetAsync<UserListItem[]>("api/User", headers);
		return _map.From<UserListItem>(users).ToArray<UserListItemModel>();
	}

	public async Task<UserModel> GetItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var user = await _client.GetAsync<User>($"api/User/{id}", headers);
		return _map.From(user).To<UserModel>();
	}

	public async Task<UserModel> GetCurrentUserAsync()
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var user = await _client.GetAsync<User>("api/User/Current", headers);
		return _map.From(user).To<UserModel>();
	}

	public async Task AddItemAsync(UserModel item)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var request = _map.From(item).To<CreateUser>();
		await _client.PostAsync("api/User", request, headers);
	}

	public async Task UpdateItemAsync(UserModel item)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var request = _map.From(item).To<UpdateUser>();
		await _client.PutAsync("api/User", request, headers);
	}

	public async Task DeleteItemAsync(Guid id)
	{
		var headers = await _tokenProvider.GetAuthorizationHeaders();
		var response = await _client.DeleteAsync($"api/User/{id}", headers);
		response.EnsureSuccessStatusCode();
	}

	public void Dispose() => _client.Dispose();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>