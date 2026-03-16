using System.Threading.Tasks;
using TemGen.Handler;
using Xunit;

namespace TemGen.Tests.Handler;

public class ReflectionHandlerTests
{
	[Fact]
	public async Task Reflection_can_read_custom_project_settings()
	{
		var project = new Project
		{
			Namespace = "TestNamespace"
		};
		project.SetSetting("UserSecretsId", "aspnet-TestNamespace.Blazor-reflection");

		var definition = new Definition
		{
			Name = "Test",
			Entries = []
		};

		var globals = new Globals
		{
			RelativePath = "Test.cs",
			Definition = definition,
			Definitions = [definition],
			DefinitionEntry = null,
			Entries = definition.Entries,
			SkipOtherDefinitions = false,
			Project = project,
			RepeatForEachDefinitionEntry = false
		};

		var handler = new ReflectionSectionHandler();

		await handler.TryHandle(globals, new TemplateSection
		{
			Handler = TemplateHandler.Reflection,
			Content = "Project.Settings.UserSecretsId"
		});

		Assert.Equal("aspnet-TestNamespace.Blazor-reflection", globals.Result);
	}
}