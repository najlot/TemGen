using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;

namespace <#cs Write(Project.Namespace)#>.Blazor.Identity;

public class MemoryUserDataStore : IUserDataStore
{
	private class UserData
	{
		public string? Username { get; set; }
		public string? Token { get; set; }
	}

	private UserData? _data;

	public MemoryUserDataStore()
	{
	}

	private Task<UserData> GetUserData()
	{
		if (_data is null)
		{
			_data = new UserData();
		}

		return Task.FromResult(_data);
	}

	private Task SaveUserData(UserData data)
	{
		_data = data;
		return Task.CompletedTask;
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