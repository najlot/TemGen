using System;
using System.IO;
using System.Linq;
using Xunit;

namespace TemGen.Tests;

public class ProjectScaffolderTests
{
	[Fact]
	public void Create_writes_a_new_project_scaffold_with_local_resources_script()
	{
		var rootDirectory = CreateRootDirectory();
		var targetDirectory = Path.Combine(rootDirectory, "Todo.Sample");

		try
		{
			var projectDirectory = ProjectScaffolder.Create(targetDirectory);
			var project = ProjectReader.ReadProject(projectDirectory);
			var definitions = DefinitionsReader.ReadDefinitions(Path.Combine(projectDirectory, "Definitions"));

			Assert.Equal(targetDirectory, projectDirectory);
			Assert.Equal("Todo.Sample", project.Namespace);
			Assert.Equal(ProjectScaffolder.DefaultTemplateSource, project.TemplatesPath);
			Assert.Equal(Path.Combine(projectDirectory, "Resources.cs"), project.ResourcesScriptPath);
			Assert.True(Directory.Exists(Path.Combine(projectDirectory, "Resources")));
			Assert.True(File.Exists(Path.Combine(projectDirectory, "ProjectDefinition.json")));
			Assert.True(File.Exists(Path.Combine(projectDirectory, "Resources.cs")));
			Assert.Contains(definitions, definition => definition.Name == "TodoItem");
			Assert.Contains(definitions, definition => definition.Name == "TodoItemStatus" && definition.IsEnumeration);
			Assert.Contains(definitions, definition => definition.Name == "User");
			Assert.Contains("Directory.GetFiles(Project.ResourcesPath", File.ReadAllText(Path.Combine(projectDirectory, "Resources.cs")));
		}
		finally
		{
			Directory.Delete(rootDirectory, true);
		}
	}

	[Fact]
	public void Create_uses_the_explicit_name_when_target_path_is_provided()
	{
		var rootDirectory = CreateRootDirectory();
		var targetDirectory = Path.Combine(rootDirectory, "generated-project");

		try
		{
			var projectDirectory = ProjectScaffolder.Create(targetDirectory, "task board");
			var project = ProjectReader.ReadProject(projectDirectory);

			Assert.Equal("TaskBoard", project.Namespace);
		}
		finally
		{
			Directory.Delete(rootDirectory, true);
		}
	}

	[Fact]
	public void Create_rejects_non_empty_definitions_directory()
	{
		var rootDirectory = CreateRootDirectory();
		var targetDirectory = Path.Combine(rootDirectory, "generated-project");
		var definitionsDirectory = Path.Combine(targetDirectory, "Definitions");

		Directory.CreateDirectory(definitionsDirectory);
		File.WriteAllText(Path.Combine(definitionsDirectory, "ExistingDefinition"), "Guid Id");

		try
		{
			var ex = Assert.Throws<InvalidOperationException>(() => ProjectScaffolder.Create(targetDirectory, "task board"));

			Assert.Contains("Definitions", ex.Message);
		}
		finally
		{
			Directory.Delete(rootDirectory, true);
		}
	}

	private static string CreateRootDirectory()
	{
		var rootDirectory = Path.Combine(Path.GetTempPath(), "TemGen.Tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(rootDirectory);
		return rootDirectory;
	}
}