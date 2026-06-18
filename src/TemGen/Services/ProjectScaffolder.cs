using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TemGen.Services;

public static class ProjectScaffolder
{
	public const string DefaultTemplateSource = "https://github.com/najlot/TemGen/tree/main/Templates/Default_Backend";
	public const string DefaultScriptsPath = "https://github.com/najlot/TemGen/tree/main/Templates/Default_Scripts";

	private const string ResourcesScriptContent = """
foreach (var path in Directory.GetFiles(Project.ResourcesPath, "*.*", SearchOption.AllDirectories))
{
	var newPath = path.Replace(Project.ResourcesPath, Project.OutputPath).Replace("Project.Namespace", Project.Namespace);
	var directoryName = Path.GetDirectoryName(newPath);
	if (!string.IsNullOrEmpty(directoryName))
	{
		Directory.CreateDirectory(directoryName);
	}

	// Validate modify time of the file, if the file is not modified, skip copying the file to save time
	var lastWriteTime = File.GetLastWriteTime(path);
	if (!File.Exists(newPath) || lastWriteTime != File.GetLastWriteTime(newPath))
	{
		File.Copy(path, newPath, true);
		File.SetLastWriteTime(newPath, lastWriteTime);
	}
}
""";

	private static readonly IReadOnlyDictionary<string, string> ExampleDefinitions = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["TodoItem"] = """
Guid Id
string Title
UserId AssignedTo
TodoItemStatus Status
DateTime? DueDate
""",
		["TodoItemStatus"] = """
Todo
InProgress
Done
""",
		["User"] = """
Guid Id
string Username
string EMail
"""
	};

	public static string Create(string targetPath, string projectName = null)
	{
		var projectDirectory = ResolveProjectDirectory(targetPath, projectName);
		var projectDefinitionPath = Path.Combine(projectDirectory, "ProjectDefinition.json");
		var definitionsDirectory = Path.Combine(projectDirectory, "Definitions");
		var resourcesDirectory = Path.Combine(projectDirectory, "Resources");
		var resourcesScriptPath = Path.Combine(projectDirectory, "Resources.cs");
		var resolvedProjectName = ResolveProjectName(projectDirectory, projectName);
		var projectNamespace = NormalizeNamespace(resolvedProjectName);

		var filesToCreate = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			[projectDefinitionPath] = BuildProjectDefinitionJson(projectNamespace),
			[resourcesScriptPath] = ResourcesScriptContent
		};

		foreach (var definition in ExampleDefinitions)
		{
			filesToCreate[Path.Combine(definitionsDirectory, definition.Key)] = definition.Value;
		}

		EnsureDirectoryIsEmpty(definitionsDirectory);
		EnsureDirectoryIsEmpty(resourcesDirectory);

		var existingFiles = filesToCreate.Keys.Where(File.Exists).OrderBy(path => path, StringComparer.OrdinalIgnoreCase).ToArray();
		if (existingFiles.Length > 0)
		{
			throw new InvalidOperationException($"Could not create a new project because the following files already exist:{Environment.NewLine}{string.Join(Environment.NewLine, existingFiles)}");
		}

		Directory.CreateDirectory(projectDirectory);
		Directory.CreateDirectory(definitionsDirectory);
		Directory.CreateDirectory(resourcesDirectory);

		foreach (var file in filesToCreate)
		{
			File.WriteAllText(file.Key, file.Value);
		}

		return projectDirectory;
	}

	private static void EnsureDirectoryIsEmpty(string path)
	{
		if (File.Exists(path))
		{
			throw new InvalidOperationException($"Could not create a new project because {path} is a file.");
		}

		if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
		{
			throw new InvalidOperationException($"Could not create a new project because {path} already exists and is not empty.");
		}
	}

	private static string ResolveProjectDirectory(string targetPath, string projectName)
	{
		if (string.IsNullOrWhiteSpace(targetPath))
		{
			targetPath = string.IsNullOrWhiteSpace(projectName) ? "." : projectName;
		}

		var fullPath = Path.GetFullPath(targetPath);

		if (fullPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
		{
			return Path.GetDirectoryName(fullPath) ?? Directory.GetCurrentDirectory();
		}

		return fullPath;
	}

	private static string ResolveProjectName(string projectDirectory, string projectName)
	{
		if (!string.IsNullOrWhiteSpace(projectName))
		{
			return projectName.Trim();
		}

		var directoryName = new DirectoryInfo(projectDirectory).Name;
		return string.IsNullOrWhiteSpace(directoryName) ? "TemGenProject" : directoryName;
	}

	private static string NormalizeNamespace(string projectName)
	{
		if (string.IsNullOrWhiteSpace(projectName))
		{
			return "TemGenProject";
		}

		var segments = projectName
			.Split(['.', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(NormalizeIdentifier)
			.Where(segment => !string.IsNullOrWhiteSpace(segment))
			.ToArray();

		return segments.Length == 0 ? "TemGenProject" : string.Join('.', segments);
	}

	private static string NormalizeIdentifier(string value)
	{
		var builder = new StringBuilder(value.Length + 1);
		var uppercaseNext = true;

		foreach (var character in value)
		{
			if (char.IsLetterOrDigit(character) || character == '_')
			{
				if (builder.Length == 0 && char.IsDigit(character))
				{
					builder.Append('N');
				}

				builder.Append(uppercaseNext ? char.ToUpperInvariant(character) : character);
				uppercaseNext = false;
			}
			else
			{
				uppercaseNext = true;
			}
		}

		return builder.ToString();
	}

	private static string BuildProjectDefinitionJson(string projectNamespace)
	{
		return $$"""
{
	"Namespace": "{{projectNamespace}}",
	"DefinitionsPath": "./Definitions",
	"TemplatesPath": "{{DefaultTemplateSource}}",
	"ScriptsPath": "{{DefaultScriptsPath}}",
	"OutputPath": "./Output",
	"ResourcesPath": "./Resources",
	"ResourcesScriptPath": "./Resources.cs",
	"PrimaryColor": "#004d40",
	"PrimaryDarkColor": "#00251a",
	"AccentColor": "#39796b",
	"ForegroundColor": "#ffffff"
}
""";
	}
}