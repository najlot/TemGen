using System.IO;
using System.Linq;
using System.Text.Json;

namespace TemGen;

public static class ProjectReader
{
	public static Project ReadProject(string path)
	{
		path = ResolveProjectPath(path);
		path = Path.GetFullPath(path);

		var project = path.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase)
			? ReadJsonProject(path)
			: ReadLegacyProject(path);

		project.ProjectDirectory = Path.GetDirectoryName(path);
		NormalizePaths(project);

		return project;
	}

	private static string ResolveProjectPath(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			path = ".";
		}

		path = Path.GetFullPath(path);

		if (Directory.Exists(path))
		{
			var jsonPath = Path.Combine(path, "ProjectDefinition.json");
			if (File.Exists(jsonPath))
			{
				return jsonPath;
			}

			return Path.Combine(path, "ProjectDefinition");
		}

		if (File.Exists(path))
		{
			return path;
		}

		if (string.IsNullOrEmpty(Path.GetExtension(path)))
		{
			var jsonPath = path + ".json";
			if (File.Exists(jsonPath))
			{
				return jsonPath;
			}
		}

		return path;
	}

	private static readonly JsonSerializerOptions _serializerOptions = new()
	{
		AllowTrailingCommas = true,
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip
	};

	private static Project ReadJsonProject(string path)
	{
		var json = File.ReadAllText(path);
		var project = JsonSerializer.Deserialize<Project>(json, _serializerOptions) ?? new Project();

		using var document = JsonDocument.Parse(json, new JsonDocumentOptions
		{
			AllowTrailingCommas = true,
			CommentHandling = JsonCommentHandling.Skip
		});

		foreach (var property in document.RootElement.EnumerateObject())
		{
			if (IsKnownJsonProperty(property.Name))
			{
				continue;
			}

			project.SetSetting(property.Name, property.Value.ValueKind == JsonValueKind.String
				? property.Value.GetString()
				: property.Value.ToString());
		}

		return project;
	}

	private static bool IsKnownJsonProperty(string propertyName)
	{
		return propertyName.Equals("Namespace", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("ProjectDirectory", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("DefinitionsPath", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("TemplatesPath", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("TemplatePaths", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("OutputPath", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("ResourcesPath", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("ResourcesScriptPath", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("ScriptsPath", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("Settings", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("PrimaryColor", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("PrimaryDarkColor", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("AccentColor", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("ForegroundColor", System.StringComparison.OrdinalIgnoreCase)
			|| propertyName.Equals("ExtensionData", System.StringComparison.OrdinalIgnoreCase);
	}

	private static Project ReadLegacyProject(string path)
	{
		var project = new Project();
		var lines = File.ReadAllLines(path).Where(p => !string.IsNullOrWhiteSpace(p) && !p.StartsWith('#'));

		foreach (var line in lines)
		{
			var index = line.IndexOf(':');

			if (index == -1)
			{
				continue;
			}

			var value = line.Substring(index + 1).Trim();

			var key = line.Substring(0, index).Trim();

			switch (key)
			{
				case "Namespace":
					project.Namespace = value;
					break;

				case "DefinitionsPath":
					project.DefinitionsPath = value;
					break;

				case "TemplatesPath":
					project.TemplatesPath = value;
					break;

				case "OutputPath":
					project.OutputPath = value;
					break;

				case "ResourcesPath":
					project.ResourcesPath = value;
					break;

				case "ResourcesScriptPath":
					project.ResourcesScriptPath = value;
					break;

				case "ScriptsPath":
					project.ScriptsPath = value;
					break;

				default:
					project.SetSetting(key, value);
					break;
			}
		}

		return project;
	}

	private static void NormalizePaths(Project project)
	{
		project.DefinitionsPath = NormalizePath(project.ProjectDirectory, project.DefinitionsPath);
		project.TemplatesPath = NormalizePathList(project.ProjectDirectory, project.TemplatesPath);
		project.OutputPath = NormalizePath(project.ProjectDirectory, project.OutputPath);
		project.ResourcesPath = NormalizePath(project.ProjectDirectory, project.ResourcesPath);
		project.ResourcesScriptPath = NormalizePath(project.ProjectDirectory, project.ResourcesScriptPath);
		project.ScriptsPath = NormalizePath(project.ProjectDirectory, project.ScriptsPath);
	}

	private static string NormalizePathList(string projectDirectory, string path)
	{
		var paths = Project.SplitPaths(path);

		if (paths.Length == 0)
		{
			return path;
		}

		return string.Join(';', paths.Select(singlePath => NormalizePath(projectDirectory, singlePath)));
	}

	private static string NormalizePath(string projectDirectory, string path)
	{
		if (string.IsNullOrWhiteSpace(path) || Path.IsPathFullyQualified(path))
		{
			return path;
		}

		return Path.GetFullPath(Path.Combine(projectDirectory, path));
	}
}