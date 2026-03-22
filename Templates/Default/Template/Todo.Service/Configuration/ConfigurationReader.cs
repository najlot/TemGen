using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace <# Project.Namespace#>.Service.Configuration;

public static class ConfigurationReader
{
	private static readonly JsonSerializerOptions _options = new()
	{
		WriteIndented = true,
		PropertyNameCaseInsensitive = true,
	};

	public static bool TryReadConfiguration<T>(this IConfiguration configuration, [NotNullWhen(true)] out T? config) where T : class, new()
	{
		var key = typeof(T).Name;
		return TryReadConfiguration<T>(configuration, key, out config);
	}

	public static bool TryReadConfiguration<T>(this IConfiguration configuration, string key, [NotNullWhen(true)] out T? config) where T : class, new()
	{
		var section = configuration.GetSection(key);

		if (!section.Exists())
		{
			return TryReadConfiguration<T>(key, out config);
		}

		var t = new T();
		section.Bind(t);
		config = t;
		return true;
	}

	public static bool TryReadConfiguration<T>(string fileName, [NotNullWhen(true)] out T? config) where T : class, new()
	{
		var configDir = "config";
		var configPath = Path.Combine(configDir, fileName);
		configPath = Path.GetFullPath(configPath);

		if (!File.Exists(configPath))
		{
			configPath += ".json";

			if (!File.Exists(configPath))
			{
				if (!Directory.Exists(configDir))
				{
					Directory.CreateDirectory(configDir);
				}

				config = null;
				return false;
			}
		}

		var configContent = File.ReadAllText(configPath);
		config = JsonSerializer.Deserialize<T>(configContent, _options) ?? new T();
		return true;
	}

	public static void WriteConfigurationExample<T>() where T : class, new()
	{
		var key = typeof(T).Name;
		WriteConfigurationExample<T>(key);
	}

	public static void WriteConfigurationExample<T>(string key) where T : class, new()
	{
		var fileName = key + ".json";
		var configDir = "config";
		var configPath = Path.Combine(configDir, fileName);
		configPath = Path.GetFullPath(configPath);

		if (!Directory.Exists(configDir))
		{
			Directory.CreateDirectory(configDir);
		}

		File.WriteAllText(configPath + ".example", JsonSerializer.Serialize(new T(), _options));
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>