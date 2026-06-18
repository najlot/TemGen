using Microsoft.JSInterop;
using <# Project.Namespace#>.Client.Data.Identity;

namespace <# Project.Namespace#>.Blazor.Identity;

public class UserDataStore : IUserDataStore
{
	private const string UsernameKey = "<# Project.Namespace#>.identity.username";
	private const string TokenKey = "<# Project.Namespace#>.identity.token";
	private readonly IJSRuntime _jsRuntime;

	public UserDataStore(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}

	public Task<string?> GetAccessToken()
	{
		return GetStorageValue(TokenKey);
	}

	public Task SetAccessToken(string token)
	{
		return SetStorageValue(TokenKey, token);
	}

	public Task<string?> GetUsername()
	{
		return GetStorageValue(UsernameKey);
	}

	public Task SetUsername(string username)
	{
		return SetStorageValue(UsernameKey, username);
	}

	public async Task SetUserData(string username, string token)
	{
		await SetStorageValue(UsernameKey, username);
		await SetStorageValue(TokenKey, token);
	}

	private async Task<string?> GetStorageValue(string key)
	{
		try
		{
			return await _jsRuntime.InvokeAsync<string?>("temgenBlazorStorage.get", key);
		}
		catch (JSException)
		{
			return null;
		}
	}

	private async Task SetStorageValue(string key, string? value)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				await _jsRuntime.InvokeVoidAsync("temgenBlazorStorage.remove", key);
				return;
			}

			await _jsRuntime.InvokeVoidAsync("temgenBlazorStorage.set", key, value);
		}
		catch (JSException)
		{
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>