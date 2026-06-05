using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;

namespace <# Project.Namespace#>.Wpf.Identity;

public class PersistentUserDataStore : IUserDataStore
{
	private sealed class UserData
	{
		public string? Username { get; set; }
		public string? Token { get; set; }
	}

	private readonly SemaphoreSlim _semaphore = new(1, 1);
	private readonly string _storagePath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
		"<# Project.Namespace#>",
		"Identity",
		"userData.dat");
	private UserData? _data;

	public async Task<string?> GetAccessToken()
	{
		var data = await GetUserData().ConfigureAwait(false);
		return data.Token;
	}

	public async Task SetAccessToken(string token)
	{
		var data = await GetUserData().ConfigureAwait(false);
		data.Token = string.IsNullOrWhiteSpace(token) ? null : token;
		await SaveUserData(data).ConfigureAwait(false);
	}

	public async Task<string?> GetUsername()
	{
		var data = await GetUserData().ConfigureAwait(false);
		return data.Username;
	}

	public async Task SetUsername(string username)
	{
		var data = await GetUserData().ConfigureAwait(false);
		data.Username = string.IsNullOrWhiteSpace(username) ? null : username;
		await SaveUserData(data).ConfigureAwait(false);
	}

	public async Task SetUserData(string username, string token)
	{
		await SaveUserData(new UserData
		{
			Username = string.IsNullOrWhiteSpace(username) ? null : username,
			Token = string.IsNullOrWhiteSpace(token) ? null : token,
		}).ConfigureAwait(false);
	}

	private async Task<UserData> GetUserData()
	{
		await _semaphore.WaitAsync().ConfigureAwait(false);
		try
		{
			if (_data is not null)
			{
				return Clone(_data);
			}

			_data = await ReadUserData().ConfigureAwait(false);
			return Clone(_data);
		}
		finally
		{
			_semaphore.Release();
		}
	}

	private async Task SaveUserData(UserData data)
	{
		await _semaphore.WaitAsync().ConfigureAwait(false);
		try
		{
			_data = Clone(data);

			if (IsEmpty(data))
			{
				TryDeleteStorage();
				return;
			}

			Directory.CreateDirectory(Path.GetDirectoryName(_storagePath)!);
			var payload = ProtectForWindows(JsonSerializer.SerializeToUtf8Bytes(data));
			await File.WriteAllBytesAsync(_storagePath, payload).ConfigureAwait(false);
		}
		catch
		{
			// Keep the in-memory cache available even when persistent storage is unavailable.
		}
		finally
		{
			_semaphore.Release();
		}
	}

	private async Task<UserData> ReadUserData()
	{
		try
		{
			if (!File.Exists(_storagePath))
			{
				return new UserData();
			}

			var payload = await File.ReadAllBytesAsync(_storagePath).ConfigureAwait(false);
			if (payload.Length == 0)
			{
				return new UserData();
			}

			payload = UnprotectForWindows(payload);
			return JsonSerializer.Deserialize<UserData>(payload) ?? new UserData();
		}
		catch
		{
			TryDeleteStorage();
			return new UserData();
		}
	}

	private static bool IsEmpty(UserData data)
		=> string.IsNullOrWhiteSpace(data.Username) && string.IsNullOrWhiteSpace(data.Token);

	private static UserData Clone(UserData data)
		=> new()
		{
			Username = data.Username,
			Token = data.Token,
		};

	private void TryDeleteStorage()
	{
		try
		{
			if (File.Exists(_storagePath))
			{
				File.Delete(_storagePath);
			}
		}
		catch
		{
		}
	}

	[SupportedOSPlatform("windows")]
	private static byte[] ProtectForWindows(byte[] payload)
		=> ProtectedData.Protect(payload, null, DataProtectionScope.CurrentUser);

	[SupportedOSPlatform("windows")]
	private static byte[] UnprotectForWindows(byte[] payload)
		=> ProtectedData.Unprotect(payload, null, DataProtectionScope.CurrentUser);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>