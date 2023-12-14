using System;
using System.IO;
using System.Linq;
using Xunit;

namespace TemGen.Tests;

public class TemplateReaderTests
{
	[Fact]
	public void TestProjectMustBeReadCorrect()
	{
		var path = Directory.GetCurrentDirectory();

		while (!Directory.GetDirectories(path).Any(p => p.Contains("TestProject")))
		{
			path = Path.GetDirectoryName(path);
		}

		var templates = TemplatesReader.ReadTemplates(Path.Combine(path, "TestProject", "Templates"));

		Assert.NotEmpty(templates);

		var templatePaths = templates.Select(t => t.RelativePath.Replace('/', '\\')).ToList();
		Assert.Contains("RootFile.cs", templatePaths);
		Assert.Contains("RootFolder\\FileInFolder.cs", templatePaths);

		var template = templates.First(t => t.RelativePath == "TestTemplate_001.cs");
		Assert.Equal(7, template.Sections.Count);

		foreach (var section in template.Sections)
		{
			if (section.Handler == TemplateHandler.CSharp)
			{
				Assert.StartsWith("Write(", section.Content);
				Assert.EndsWith(")", section.Content);
			}
		}
	}


	[Fact]
	public void ReadResourceScript_must_select_correct_path_for_relative_resources_path()
	{
		// Arrange
		var content = "Console.WriteLine(123);";
		var testId = Guid.NewGuid().ToString();

		var directory = Path.Combine(".", testId);
		var filename = "res.cs";
		var path = Path.Combine(directory, filename);

		Directory.CreateDirectory(directory);

		File.WriteAllText(path, content);

		// Act
		var template = TemplatesReader.ReadResourceScript(".", path);

		// Cleanup
		Directory.Delete(directory, true);

		// Assert
		Assert.Equal(Path.Combine(testId, filename), template.RelativePath);

		Assert.Single(template.Sections);
		Assert.Equal(TemplateHandler.CSharp, template.Sections[0].Handler);
		Assert.Equal(content, template.Sections[0].Content);
	}

	[Fact]
	public void ReadResourceScript_must_select_correct_path_for_absolute_path()
	{
		// Arrange
		var content = "Console.WriteLine(123);";
		var testId = Guid.NewGuid().ToString();

		var directory = Path.Combine(".", testId);
		var filename = "res.cs";
		var path = Path.Combine(directory, filename);

		Directory.CreateDirectory(directory);

		File.WriteAllText(path, content);

		// Act
		var template = TemplatesReader.ReadResourceScript(Directory.GetCurrentDirectory(), path);

		// Cleanup
		Directory.Delete(directory, true);

		// Assert
		Assert.Equal(Path.Combine(testId, filename), template.RelativePath);

		Assert.Single(template.Sections);
		Assert.Equal(TemplateHandler.CSharp, template.Sections[0].Handler);
		Assert.Equal(content, template.Sections[0].Content);
	}

	[Fact]
	public void ReadResourceScript_must_select_correct_path_for_absolute_resources_path()
	{
		// Arrange
		var content = "Console.WriteLine(123);";
		var testId = Guid.NewGuid().ToString();

		var directory = Path.Combine(Directory.GetCurrentDirectory(), testId);
		var filename = "res.cs";
		var path = Path.Combine(directory, filename);

		Directory.CreateDirectory(directory);

		File.WriteAllText(path, content);

		// Act
		var template = TemplatesReader.ReadResourceScript(".", path);

		// Cleanup
		Directory.Delete(directory, true);

		// Assert
		Assert.Equal(Path.Combine(testId, filename), template.RelativePath);

		Assert.Single(template.Sections);
		Assert.Equal(TemplateHandler.CSharp, template.Sections[0].Handler);
		Assert.Equal(content, template.Sections[0].Content);
	}

	[Fact]
	public void ReadResourceScript_must_select_correct_path_for_absolute_paths()
	{
		// Arrange
		var content = "Console.WriteLine(123);";
		var testId = Guid.NewGuid().ToString();

		var directory = Path.Combine(Directory.GetCurrentDirectory(), testId);
		var filename = "res.cs";
		var path = Path.Combine(directory, filename);

		Directory.CreateDirectory(directory);

		File.WriteAllText(path, content);

		// Act
		var template = TemplatesReader.ReadResourceScript(Directory.GetCurrentDirectory(), path);

		// Cleanup
		Directory.Delete(directory, true);

		// Assert
		Assert.Equal(Path.Combine(testId, filename), template.RelativePath);

		Assert.Single(template.Sections);
		Assert.Equal(TemplateHandler.CSharp, template.Sections[0].Handler);
		Assert.Equal(content, template.Sections[0].Content);
	}
}