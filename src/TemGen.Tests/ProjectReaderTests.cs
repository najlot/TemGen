using System;
using System.IO;
using Xunit;

namespace TemGen.Tests;

public class ProjectReaderTests
{
	[Fact]
	public void Project_returns_default_value_for_missing_setting()
	{
		var project = new Project
		{
			Namespace = "LegacyProject"
		};

		Assert.Equal("aspnet-LegacyProject.Blazor-4ea02e08-91a1-464f-b410-a492610d37b0", project.GetSetting("UserSecretsId", $"aspnet-{project.Namespace}.Blazor-4ea02e08-91a1-464f-b410-a492610d37b0"));

		project.SetSetting("UserSecretsId", "custom-secret-id");

		Assert.Equal("custom-secret-id", project.GetSetting("UserSecretsId", $"aspnet-{project.Namespace}.Blazor-4ea02e08-91a1-464f-b410-a492610d37b0"));
	}

	[Fact]
	public void ReadProject_prefers_json_project_definition_when_present()
	{
		var projectDirectory = CreateProjectDirectory();

		try
		{
			File.WriteAllText(Path.Combine(projectDirectory, "ProjectDefinition.json"), """
{
  "Namespace": "JsonProject",
  "DefinitionsPath": "Definitions",
  "TemplatesPath": "Templates",
  "OutputPath": "Output",
  "PrimaryColor": "#111111",
  "PrimaryDarkColor": "#222222",
  "AccentColor": "#333333",
	"ForegroundColor": "#444444",
	"UserSecretsId": "aspnet-JsonProject.Blazor-test"
}
""");

			File.WriteAllText(Path.Combine(projectDirectory, "ProjectDefinition"), """
Namespace: LegacyProject
DefinitionsPath: LegacyDefinitions
TemplatesPath: LegacyTemplates
OutputPath: LegacyOutput
PrimaryColor: #aaaaaa
""");

			var project = ProjectReader.ReadProject(projectDirectory);

			Assert.Equal("JsonProject", project.Namespace);
			Assert.Equal(projectDirectory, project.ProjectDirectory);
			Assert.Equal(Path.Combine(projectDirectory, "Definitions"), project.DefinitionsPath);
			Assert.Equal(Path.Combine(projectDirectory, "Templates"), project.TemplatesPath);
			Assert.Equal(Path.Combine(projectDirectory, "Output"), project.OutputPath);
			Assert.Equal("#111111", project.PrimaryColor);
			Assert.Equal("#222222", project.PrimaryDarkColor);
			Assert.Equal("#333333", project.AccentColor);
			Assert.Equal("#444444", project.ForegroundColor);
			Assert.Equal("#111111", project.GetSetting("PrimaryColor"));
			Assert.Equal("aspnet-JsonProject.Blazor-test", project.GetSetting("UserSecretsId"));
		}
		finally
		{
			Directory.Delete(projectDirectory, true);
		}
	}

	[Fact]
	public void ReadProject_falls_back_to_legacy_project_definition_when_json_is_missing()
	{
		var projectDirectory = CreateProjectDirectory();

		try
		{
			File.WriteAllText(Path.Combine(projectDirectory, "ProjectDefinition"), """
Namespace: LegacyProject
DefinitionsPath: Definitions
TemplatesPath: Templates
OutputPath: Output
ResourcesPath: Resources
ResourcesScriptPath: scripts/resources.cs
ScriptsPath: scripts
PrimaryColor: #306090
PrimaryDarkColor: #003762
AccentColor: #638dc1
ForegroundColor: #ffffff
UserSecretsId: aspnet-LegacyProject.Blazor-test
""");

			var project = ProjectReader.ReadProject(projectDirectory);

			Assert.Equal("LegacyProject", project.Namespace);
			Assert.Equal(projectDirectory, project.ProjectDirectory);
			Assert.Equal(Path.Combine(projectDirectory, "Definitions"), project.DefinitionsPath);
			Assert.Equal(Path.Combine(projectDirectory, "Templates"), project.TemplatesPath);
			Assert.Equal(Path.Combine(projectDirectory, "Output"), project.OutputPath);
			Assert.Equal(Path.Combine(projectDirectory, "Resources"), project.ResourcesPath);
			Assert.Equal(Path.Combine(projectDirectory, "scripts", "resources.cs"), project.ResourcesScriptPath);
			Assert.Equal(Path.Combine(projectDirectory, "scripts"), project.ScriptsPath);
			Assert.Equal("#306090", project.PrimaryColor);
			Assert.Equal("#003762", project.PrimaryDarkColor);
			Assert.Equal("#638dc1", project.AccentColor);
			Assert.Equal("#ffffff", project.ForegroundColor);
			Assert.Equal("#638dc1", project.GetSetting("AccentColor"));
			Assert.Equal("aspnet-LegacyProject.Blazor-test", project.GetSetting("UserSecretsId"));
		}
		finally
		{
			Directory.Delete(projectDirectory, true);
		}
	}

	[Fact]
	public void ReadProject_reads_nested_json_settings_case_insensitively()
	{
		var projectDirectory = CreateProjectDirectory();

		try
		{
			File.WriteAllText(Path.Combine(projectDirectory, "ProjectDefinition.json"), """
{
  "Namespace": "JsonProject",
  "DefinitionsPath": "Definitions",
  "TemplatesPath": "Templates",
  "OutputPath": "Output",
  "Settings": {
    "UserSecretsId": "aspnet-JsonProject.Blazor-settings",
    "primarycolor": "#abcdef"
  }
}
""");

			var project = ProjectReader.ReadProject(projectDirectory);

			Assert.Equal("aspnet-JsonProject.Blazor-settings", project.GetSetting("UserSecretsId"));
			Assert.Equal("#abcdef", project.PrimaryColor);
			Assert.Equal("#abcdef", project.GetSetting("PrimaryColor"));
		}
		finally
		{
			Directory.Delete(projectDirectory, true);
		}
	}

	[Fact]
	public void ReadProject_normalizes_multiple_template_paths_in_order()
	{
		var projectDirectory = CreateProjectDirectory();

		try
		{
			Directory.CreateDirectory(Path.Combine(projectDirectory, "Templates.Override"));

			File.WriteAllText(Path.Combine(projectDirectory, "ProjectDefinition.json"), """
{
  "Namespace": "JsonProject",
  "DefinitionsPath": "Definitions",
  "TemplatesPath": "Templates; Templates.Override ",
  "OutputPath": "Output"
}
""");

			var project = ProjectReader.ReadProject(projectDirectory);

			Assert.Equal(
				$"{Path.Combine(projectDirectory, "Templates")};{Path.Combine(projectDirectory, "Templates.Override")}",
				project.TemplatesPath);
			Assert.Equal(
				[
					Path.Combine(projectDirectory, "Templates"),
					Path.Combine(projectDirectory, "Templates.Override")
				],
				project.TemplatePaths);
		}
		finally
		{
			Directory.Delete(projectDirectory, true);
		}
	}

	private static string CreateProjectDirectory()
	{
		var projectDirectory = Path.Combine(Path.GetTempPath(), "TemGen.Tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(Path.Combine(projectDirectory, "Definitions"));
		Directory.CreateDirectory(Path.Combine(projectDirectory, "Templates"));
		Directory.CreateDirectory(Path.Combine(projectDirectory, "Output"));
		Directory.CreateDirectory(Path.Combine(projectDirectory, "Resources"));
		Directory.CreateDirectory(Path.Combine(projectDirectory, "scripts"));
		return projectDirectory;
	}
}