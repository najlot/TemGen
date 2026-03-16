using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TemGen;

public class Project
{
	public string Namespace { get; set; }

	public string ProjectDirectory { get; set; }
	public string DefinitionsPath { get; set; }
	public string TemplatesPath { get; set; }
	public string[] TemplatePaths => SplitPaths(TemplatesPath);
	public string OutputPath { get; set; }
	public string ResourcesPath { get; set; }
	public string ResourcesScriptPath { get; set; }
	public string ScriptsPath { get; set; }
	public Dictionary<string, string> Settings
	{
		get;
		set
		{
			if (value is null)
			{
				field = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
			}
			else
			{
				field = new Dictionary<string, string>(value, System.StringComparer.OrdinalIgnoreCase);
			}
		}
	} = new(System.StringComparer.OrdinalIgnoreCase);

	[JsonExtensionData]
	public Dictionary<string, JsonElement> ExtensionData
	{
		get;
		set
		{
			field = value;

			if (value is null)
			{
				return;
			}

			foreach (var setting in value)
			{
				if (setting.Value.ValueKind == JsonValueKind.String)
				{
					SetSetting(setting.Key, setting.Value.GetString());
				}
				else
				{
					SetSetting(setting.Key, setting.Value.ToString());
				}
			}
		}
	}

	public string PrimaryColor
	{
		get => GetSetting(nameof(PrimaryColor));
		set => SetSetting(nameof(PrimaryColor), value);
	}

	public string PrimaryDarkColor
	{
		get => GetSetting(nameof(PrimaryDarkColor));
		set => SetSetting(nameof(PrimaryDarkColor), value);
	}

	public string AccentColor
	{
		get => GetSetting(nameof(AccentColor));
		set => SetSetting(nameof(AccentColor), value);
	}

	public string ForegroundColor
	{
		get => GetSetting(nameof(ForegroundColor));
		set => SetSetting(nameof(ForegroundColor), value);
	}

	public string this[string key]
	{
		get => GetSetting(key);
		set => SetSetting(key, value);
	}

	public string GetSetting(string key)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			return "";
		}

		return Settings.TryGetValue(key, out var value) ? value : "";
	}

	public string GetSetting(string key, string defaultValue)
	{
		return TryGetSetting(key, out var value) ? value : defaultValue;
	}

	public bool TryGetSetting(string key, out string value)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			value = string.Empty;
			return false;
		}

		return Settings.TryGetValue(key, out value);
	}

	public void SetSetting(string key, string value)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			return;
		}

		if (value is null)
		{
			Settings.Remove(key);
			return;
		}

		Settings[key] = value;
	}

	public static string[] SplitPaths(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return [];
		}

		return path.Split(';', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
	}
}