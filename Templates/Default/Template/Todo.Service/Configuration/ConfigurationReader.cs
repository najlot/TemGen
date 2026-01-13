using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Najlot.Log;

namespace <#cs Write(Project.Namespace)#>.Service.Configuration;

public static class ConfigurationReader
{
	private static readonly JsonSerializerOptions _options = new()
	{
		WriteIndented = true,
		PropertyNameCaseInsensitive = true,
	};

	public static T? ReadConfiguration<T>(this IConfiguration configuration) where T : class, new()
	{
		var key = typeof(T).Name;
		return ReadConfiguration<T>(configuration, key);
	}

	public static T? ReadConfiguration<T>(this IConfiguration configuration, string key) where T : class, new()
	{
		var section = configuration.GetSection(key);

		if (!section.Exists())
		{
			return ReadConfiguration<T>(key);
		}

		var t = new T();
		section.Bind(t);
		return t;
	}

	public static T? ReadConfiguration<T>(string fileName) where T : class, new()
	{
		var configDir = "config";
		var configPath = Path.Combine(configDir, fileName);
		configPath = Path.GetFullPath(configPath);

		if (!File.Exists(configPath))
		{
			configPath += ".json";

			if (!File.Exists(configPath))
			{
				if (!File.Exists(configPath + ".example"))
				{
					if (!Directory.Exists(configDir))
					{
						Directory.CreateDirectory(configDir);
					}

					File.WriteAllText(configPath + ".example", JsonSerializer.Serialize(new T(), _options));
				}

				return null;
			}
		}

		var configContent = File.ReadAllText(configPath);
		return JsonSerializer.Deserialize<T>(configContent, _options);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>