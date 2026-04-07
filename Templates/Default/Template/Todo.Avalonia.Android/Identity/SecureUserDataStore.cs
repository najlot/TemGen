using Android.Content;
using Android.Security.Keystore;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;

namespace <# Project.Namespace#>.Avalonia.Android.Identity;

public class SecureUserDataStore : IUserDataStore
{
	private sealed class UserData
	{
		public string? Username { get; set; }
		public string? Token { get; set; }
	}

	private sealed class ProtectedPayload
	{
		public string? Iv { get; set; }
		public string? CipherText { get; set; }
	}

	private const string AndroidKeyStoreName = "AndroidKeyStore";
	private const string CipherTransformation = "AES/GCM/NoPadding";
	private const string PreferencesName = "<# Project.Namespace#>.identity.secure";
	private const string StorageKey = "user_data";
	private const int TagLengthBits = 128;
	private static readonly string KeyAlias = "<# Project.Namespace#>.identity.key";

	private readonly ISharedPreferences _sharedPreferences;
	private readonly SemaphoreSlim _semaphore = new(1, 1);
	private UserData? _data;

	public SecureUserDataStore(Context context)
	{
		var applicationContext = context.ApplicationContext ?? context;
		_sharedPreferences = applicationContext.GetSharedPreferences(PreferencesName, FileCreationMode.Private)
			?? throw new InvalidOperationException("Secure user data preferences could not be created.");
	}

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

			_data = ReadUserData();
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
				ClearStoredUserData();
				return;
			}

			var payload = Encrypt(data);
			var editor = _sharedPreferences.Edit()
				?? throw new InvalidOperationException("Secure user data editor could not be created.");
			editor.PutString(StorageKey, payload);
			editor.Commit();
		}
		catch
		{
			// Keep the in-memory cache available even when secure persistence is temporarily unavailable.
		}
		finally
		{
			_semaphore.Release();
		}
	}

	private UserData ReadUserData()
	{
		try
		{
			var protectedPayload = _sharedPreferences.GetString(StorageKey, null);
			if (string.IsNullOrWhiteSpace(protectedPayload))
			{
				return new UserData();
			}

			return Decrypt(protectedPayload);
		}
		catch
		{
			ClearStoredUserData();
			DeleteKeyIfPresent();
			return new UserData();
		}
	}

	private string Encrypt(UserData data)
	{
		var key = GetOrCreateSecretKey();
		using var cipher = Cipher.GetInstance(CipherTransformation)
			?? throw new InvalidOperationException("Android encryption cipher could not be created.");
		cipher.Init(CipherMode.EncryptMode, key);

		var payload = JsonSerializer.SerializeToUtf8Bytes(data);
		var cipherText = cipher.DoFinal(payload)
			?? throw new InvalidOperationException("Android encryption did not return cipher text.");
		var iv = cipher.GetIV()
			?? throw new InvalidOperationException("Android encryption did not return an initialization vector.");

		var protectedPayload = new ProtectedPayload
		{
			Iv = Convert.ToBase64String(iv),
			CipherText = Convert.ToBase64String(cipherText),
		};

		return JsonSerializer.Serialize(protectedPayload);
	}

	private UserData Decrypt(string protectedPayload)
	{
		var payload = JsonSerializer.Deserialize<ProtectedPayload>(protectedPayload) ?? new ProtectedPayload();
		if (string.IsNullOrWhiteSpace(payload.Iv) || string.IsNullOrWhiteSpace(payload.CipherText))
		{
			return new UserData();
		}

		var key = GetOrCreateSecretKey();
		using var cipher = Cipher.GetInstance(CipherTransformation)
			?? throw new InvalidOperationException("Android decryption cipher could not be created.");
		using var parameterSpec = new GCMParameterSpec(TagLengthBits, Convert.FromBase64String(payload.Iv));
		cipher.Init(CipherMode.DecryptMode, key, parameterSpec);

		var userDataBytes = cipher.DoFinal(Convert.FromBase64String(payload.CipherText))
			?? throw new InvalidOperationException("Android decryption did not return plaintext.");
		return JsonSerializer.Deserialize<UserData>(userDataBytes) ?? new UserData();
	}

	private static bool IsEmpty(UserData data)
		=> string.IsNullOrWhiteSpace(data.Username) && string.IsNullOrWhiteSpace(data.Token);

	private static UserData Clone(UserData data)
		=> new()
		{
			Username = data.Username,
			Token = data.Token,
		};

	private void ClearStoredUserData()
	{
		var editor = _sharedPreferences.Edit()
			?? throw new InvalidOperationException("Secure user data editor could not be created.");
		editor.Remove(StorageKey);
		editor.Commit();
	}

	private static IKey GetOrCreateSecretKey()
	{
		var keyStore = KeyStore.GetInstance(AndroidKeyStoreName)
			?? throw new InvalidOperationException("Android keystore could not be opened.");
		keyStore.Load(null);

		if (!keyStore.ContainsAlias(KeyAlias))
		{
			GenerateSecretKey();
			keyStore.Load(null);
		}

		return keyStore.GetKey(KeyAlias, null)
			?? throw new InvalidOperationException("Android secure storage key could not be resolved.");
	}

	private static void GenerateSecretKey()
	{
		using var keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, AndroidKeyStoreName)
			?? throw new InvalidOperationException("Android key generator could not be created.");
		var keySpec = new KeyGenParameterSpec.Builder(
			KeyAlias,
			KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
			.SetBlockModes(KeyProperties.BlockModeGcm)
			.SetEncryptionPaddings(KeyProperties.EncryptionPaddingNone)
			.SetRandomizedEncryptionRequired(true)
			.Build();

		keyGenerator.Init(keySpec);
		keyGenerator.GenerateKey();
	}

	private static void DeleteKeyIfPresent()
	{
		try
		{
			var keyStore = KeyStore.GetInstance(AndroidKeyStoreName);
			if (keyStore is null)
			{
				return;
			}

			keyStore.Load(null);
			if (keyStore.ContainsAlias(KeyAlias))
			{
				keyStore.DeleteEntry(KeyAlias);
			}
		}
		catch
		{
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>