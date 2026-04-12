using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Todo.Client.Data.Identity;

namespace Todo.Avalonia.Browser.Identity;

public class WebUserDataStore : IUserDataStore
{
	private const string UsernameKey = "Todo.identity.username";
	private const string TokenKey = "Todo.identity.token";

	public Task<string?> GetAccessToken()
	{
		return Task.FromResult(BrowserRuntimeConfiguration.GetLocalStorage(TokenKey));
	}

	public Task<string?> GetUsername()
	{
		return Task.FromResult(BrowserRuntimeConfiguration.GetLocalStorage(UsernameKey));
	}

	public Task SetAccessToken(string token)
	{
		BrowserRuntimeConfiguration.SetLocalStorage(TokenKey, token);
		return Task.CompletedTask;
	}

	public Task SetUserData(string username, string token)
	{
		BrowserRuntimeConfiguration.SetLocalStorage(UsernameKey, username);
		BrowserRuntimeConfiguration.SetLocalStorage(TokenKey, token);
		return Task.CompletedTask;
	}

	public Task SetUsername(string username)
	{
		BrowserRuntimeConfiguration.SetLocalStorage(UsernameKey, username);
		return Task.CompletedTask;
	}
}

internal static partial class BrowserRuntimeConfiguration
{
	public static string GetDataServiceUrl()
	{
		var protocol = GetProtocol();
		var hostname = GetHostName();

		if (string.IsNullOrWhiteSpace(protocol))
		{
			protocol = "http:";
		}

		if (string.IsNullOrWhiteSpace(hostname))
		{
			hostname = "localhost";
		}

		return $"{protocol}//{hostname}:5000/";
	}

	public static string? GetLocalStorage(string key)
	{
		try
		{
			return LocalStorageGetItem(key);
		}
		catch (JSException)
		{
			return null;
		}
	}

	public static void SetLocalStorage(string key, string? value)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				LocalStorageRemoveItem(key);
				return;
			}

			LocalStorageSetItem(key, value);
		}
		catch (JSException)
		{
		}
	}

	[JSImport("globalThis.temgenBrowserGetLocalStorage")]
	private static partial string? LocalStorageGetItem(string key);

	[JSImport("globalThis.temgenBrowserSetLocalStorage")]
	private static partial void LocalStorageSetItem(string key, string value);

	[JSImport("globalThis.temgenBrowserRemoveLocalStorage")]
	private static partial void LocalStorageRemoveItem(string key);

	[JSImport("globalThis.temgenBrowserGetProtocol")]
	private static partial string GetProtocol();

	[JSImport("globalThis.temgenBrowserGetHostName")]
	private static partial string GetHostName();
}
