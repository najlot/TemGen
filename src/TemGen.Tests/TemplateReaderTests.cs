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
}