using System.IO;
using System.Linq;
using Xunit;

namespace TemGen.Tests;

public class ProjectReaderTests
{
	[Fact]
	public void TestProjectMustBeReadCorrect()
	{
		var path = Directory.GetCurrentDirectory();

		while (!Directory.GetDirectories(path).Any(p => p.Contains("TestProject")))
		{
			path = Path.GetDirectoryName(path);
		}

		var project = ProjectReader.ReadProject(Path.Combine(path, "TestProject"));

		Assert.Equal("TestProject", project.Namespace);

		Assert.NotNull(project.ProjectDirectory);
		Assert.NotEmpty(project.ProjectDirectory);

		Assert.DoesNotContain("./", project.DefinitionsPath);
		Assert.Contains("Definitions", project.DefinitionsPath);
		Assert.Contains("Templates", project.TemplatesPath);
		Assert.Contains("Output", project.OutputPath);

		Assert.True(Directory.Exists(project.DefinitionsPath));
		Assert.True(Directory.Exists(project.TemplatesPath));

		Assert.Equal("#306090", project.PrimaryColor);
		Assert.Equal("#003762", project.PrimaryDarkColor);
		Assert.Equal("#638dc1", project.AccentColor);
		Assert.Equal("#ffffff", project.ForegroundColor);
	}
}