using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;

namespace <#cs Write(Project.Namespace)#>.Blazor.Identity;

public class UserDataStore : IUserDataStore
{
	private class UserData
	{
		public string? Username { get; set; }
		public string? Token { get; set; }
	}

	private const string StorageName = "user_data";
	private UserData? _data;
	private readonly ProtectedLocalStorage _protectedLocalStorage;
	private readonly ILogger<UserDataStore> _log;

	public UserDataStore(ProtectedLocalStorage protectedLocalStorage, ILogger<UserDataStore> log)
	{
		_protectedLocalStorage = protectedLocalStorage;
		_log = log;
	}

	private async Task<UserData> GetUserData()
	{
		if (_data is null)
		{
			try
			{
				var result = await _protectedLocalStorage.GetAsync<UserData>(StorageName);

				if (result.Success)
				{
					_data = result.Value;
				}
			}
			catch (Exception ex)
			{
				_log.LogError(ex, "Could not access ProtectedLocalStorage");
			}
		}

		_data ??= new UserData();

		return _data;
	}

	private async Task SaveUserData(UserData data)
	{
		_data = data;
		await _protectedLocalStorage.SetAsync(StorageName, _data);
	}

	public async Task<string?> GetAccessToken()
	{
		var data = await GetUserData();
		return data.Token;
	}

	public async Task SetAccessToken(string token)
	{
		var data = await GetUserData();
		data.Token = token;
		await SaveUserData(data);
	}

	public async Task<string?> GetUsername()
	{
		var data = await GetUserData();
		return data.Username;
	}

	public async Task SetUsername(string username)
	{
		var data = await GetUserData();
		data.Username = username;
		await SaveUserData(data);
	}

	public async Task SetUserData(string username, string token)
	{
		var data = await GetUserData();
		data.Username = username;
		data.Token = token;
		await SaveUserData(data);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>