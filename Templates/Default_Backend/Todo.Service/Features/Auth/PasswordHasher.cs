using System.Buffers.Binary;
using System.Security.Cryptography;

namespace <# Project.Namespace#>.Service.Features.Auth;

public static class PasswordHasher
{
	private const byte FormatMarker = 1;
	private const int Iterations = 100_000;
	private const int SaltSize = 16;
	private const int HashSize = 32;
	private const int HeaderSize = 1 + sizeof(int);
	private const int LegacySha256HashSize = 32;

	public static byte[] HashPassword(string password)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(password);

		var salt = RandomNumberGenerator.GetBytes(SaltSize);
		var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
		var payload = new byte[HeaderSize + SaltSize + HashSize];

		payload[0] = FormatMarker;
		BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(1, sizeof(int)), Iterations);
		salt.CopyTo(payload.AsSpan(HeaderSize, SaltSize));
		hash.CopyTo(payload.AsSpan(HeaderSize + SaltSize, HashSize));

		return payload;
	}

	public static bool VerifyPassword(string password, byte[]? passwordHash)
	{
		if (string.IsNullOrWhiteSpace(password) || passwordHash is not { Length: > 0 })
		{
			return false;
		}

		return IsCurrentFormat(passwordHash)
			? VerifyCurrentFormat(password, passwordHash)
			: VerifyLegacyHash(password, passwordHash);
	}

	private static bool IsCurrentFormat(byte[] passwordHash)
	{
		return passwordHash.Length == HeaderSize + SaltSize + HashSize && passwordHash[0] == FormatMarker;
	}

	private static bool VerifyCurrentFormat(string password, byte[] passwordHash)
	{
		var iterations = BinaryPrimitives.ReadInt32LittleEndian(passwordHash.AsSpan(1, sizeof(int)));
		if (iterations <= 0)
		{
			return false;
		}

		var salt = passwordHash.AsSpan(HeaderSize, SaltSize);
		var expectedHash = passwordHash.AsSpan(HeaderSize + SaltSize, HashSize);
		var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, HashSize);

		return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
	}

	private static bool VerifyLegacyHash(string password, byte[] passwordHash)
	{
		if (passwordHash.Length != LegacySha256HashSize)
		{
			return false;
		}

		var actualHash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password));
		return CryptographicOperations.FixedTimeEquals(actualHash, passwordHash);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>