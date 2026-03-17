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
		var template = TemplatesReader.ReadScript(".", path);

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
		var template = TemplatesReader.ReadScript(Directory.GetCurrentDirectory(), path);

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
		var template = TemplatesReader.ReadScript(".", path);

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
		var template = TemplatesReader.ReadScript(Directory.GetCurrentDirectory(), path);

		// Cleanup
		Directory.Delete(directory, true);

		// Assert
		Assert.Equal(Path.Combine(testId, filename), template.RelativePath);

		Assert.Single(template.Sections);
		Assert.Equal(TemplateHandler.CSharp, template.Sections[0].Handler);
		Assert.Equal(content, template.Sections[0].Content);
	}

	[Fact]
	public void ReadTemplates_must_build_nested_csharp_control_flow_sections()
	{
		var testId = Guid.NewGuid().ToString();
		var directory = Path.Combine(".", testId);
		var path = Path.Combine(directory, "NestedTemplate.txt");
		var content = "<#csfor entry in Entries.Take(1)\r\n#><#csif entry.Field == \"Name\"#>A<#csElseIf entry.Field == \"Title\"#>B<#else#>C<#end#><#end#>";

		Directory.CreateDirectory(directory);
		File.WriteAllText(path, content);

		try
		{
			var template = TemplatesReader.ReadTemplates(directory).Single();
			var rootSections = template.Sections.Where(section => section.Handler != TemplateHandler.Text || !string.IsNullOrEmpty(section.Content)).ToList();
			var nestedSections = rootSections[0].Sections.Where(section => section.Handler != TemplateHandler.Text || !string.IsNullOrEmpty(section.Content)).ToList();

			Assert.Single(rootSections);
			Assert.Equal(TemplateHandler.CSharpFor, rootSections[0].Handler);
			Assert.Equal("entry in Entries.Take(1)", rootSections[0].Content.Trim());
			Assert.Single(nestedSections);
			Assert.Equal(TemplateHandler.CSharpIf, nestedSections[0].Handler);
			Assert.Equal("entry.Field == \"Name\"", nestedSections[0].Content.Trim());
			Assert.Single(nestedSections[0].Sections);
			Assert.Single(nestedSections[0].ElseSections);
			Assert.Equal("A", nestedSections[0].Sections[0].Content);
			Assert.Equal(TemplateHandler.CSharpElseIf, nestedSections[0].ElseSections[0].Handler);
			Assert.Equal("entry.Field == \"Title\"", nestedSections[0].ElseSections[0].Content.Trim());
			Assert.Single(nestedSections[0].ElseSections[0].Sections);
			Assert.Equal("B", nestedSections[0].ElseSections[0].Sections[0].Content);
			Assert.Single(nestedSections[0].ElseSections[0].ElseSections);
			Assert.Equal("C", nestedSections[0].ElseSections[0].ElseSections[0].Content);
		}
		finally
		{
			Directory.Delete(directory, true);
		}
	}

	[Fact]
	public void ReadTemplates_must_support_control_flow_aliases_without_cs_prefix()
	{
		var testId = Guid.NewGuid().ToString();
		var directory = Path.Combine(".", testId);
		var path = Path.Combine(directory, "AliasTemplate.txt");
		var content = "<#for entry in Entries.Take(1)\r\n#><#if entry.Field == \"Name\"#>A<#elseif entry.Field == \"Title\"#>B<#else#>C<#end#><#end#>";

		Directory.CreateDirectory(directory);
		File.WriteAllText(path, content);

		try
		{
			var template = TemplatesReader.ReadTemplates(directory).Single();
			var rootSections = template.Sections.Where(section => section.Handler != TemplateHandler.Text || !string.IsNullOrEmpty(section.Content)).ToList();
			var nestedSections = rootSections[0].Sections.Where(section => section.Handler != TemplateHandler.Text || !string.IsNullOrEmpty(section.Content)).ToList();

			Assert.Single(rootSections);
			Assert.Equal(TemplateHandler.CSharpFor, rootSections[0].Handler);
			Assert.Equal("entry in Entries.Take(1)", rootSections[0].Content.Trim());
			Assert.Single(nestedSections);
			Assert.Equal(TemplateHandler.CSharpIf, nestedSections[0].Handler);
			Assert.Equal("entry.Field == \"Name\"", nestedSections[0].Content.Trim());
			Assert.Single(nestedSections[0].ElseSections);
			Assert.Equal(TemplateHandler.CSharpElseIf, nestedSections[0].ElseSections[0].Handler);
			Assert.Equal("entry.Field == \"Title\"", nestedSections[0].ElseSections[0].Content.Trim());
		}
		finally
		{
			Directory.Delete(directory, true);
		}
	}

	[Fact]
	public void ReadTemplates_must_default_empty_language_tags_to_reflection()
	{
		var testId = Guid.NewGuid().ToString();
		var directory = Path.Combine(".", testId);
		var path = Path.Combine(directory, "ReflectionDefaultTemplate.txt");
		var content = "Hello <# Project.Namespace#>";

		Directory.CreateDirectory(directory);
		File.WriteAllText(path, content);

		try
		{
			var template = TemplatesReader.ReadTemplates(directory).Single();
			var reflectionSection = template.Sections.Single(section => section.Handler == TemplateHandler.Reflection);

			Assert.Equal("Project.Namespace", reflectionSection.Content);
		}
		finally
		{
			Directory.Delete(directory, true);
		}
	}

	[Fact]
	public void ReadTemplates_must_parse_whitespace_only_empty_language_tags_as_reflection()
	{
		var testId = Guid.NewGuid().ToString();
		var directory = Path.Combine(".", testId);
		var path = Path.Combine(directory, "EmptyTemplate.txt");
		var content = "A<#   #>B";

		Directory.CreateDirectory(directory);
		File.WriteAllText(path, content);

		try
		{
			var template = TemplatesReader.ReadTemplates(directory).Single();
			var reflectionSection = template.Sections.Single(section => section.Handler == TemplateHandler.Reflection);
			Assert.True(string.IsNullOrWhiteSpace(reflectionSection.Content));
		}
		finally
		{
			Directory.Delete(directory, true);
		}
	}

	[Fact]
	public void ReadTemplates_must_include_relative_template_path_when_parsing_fails()
	{
		var testId = Guid.NewGuid().ToString();
		var directory = Path.Combine(".", testId);
		var nestedDirectory = Path.Combine(directory, "Nested");
		var path = Path.Combine(nestedDirectory, "BrokenTemplate.txt");

		Directory.CreateDirectory(nestedDirectory);
		File.WriteAllText(path, "<#csfor entry in Entries#>broken");

		try
		{
			var ex = Assert.Throws<AggregateException>(() => TemplatesReader.ReadTemplates(directory));
			var templateError = Assert.Single(ex.InnerExceptions);

			Assert.IsType<TemplateReadException>(templateError);
			Assert.Contains(Path.Combine("Nested", "BrokenTemplate.txt"), templateError.Message);
			Assert.Contains("Unclosed <#csfor#> block", templateError.Message);
		}
		finally
		{
			Directory.Delete(directory, true);
		}
	}
}