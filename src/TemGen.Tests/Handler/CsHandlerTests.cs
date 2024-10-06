using System.Threading.Tasks;
using TemGen.Handler;
using Xunit;

namespace TemGen.Tests.Handler;

public class CsHandlerTests
{
	[Fact]
	public async Task CsMustBeRunCorrect()
	{
		var project = new Project()
		{
			Namespace = "TestNamespace",
		};

		var definition = new Definition()
		{
			Name = "Test",
			Entries =
		[
			new DefinitionEntry()
			{
				Field = "Entry_1"
			},
			new DefinitionEntry()
			{
				Field = "Entry_2"
			},
		]
		};

		var globals = new Globals()
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

		var handler = new CsSectionHandler();
		await handler.Handle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "Write(Definition.Name + ':'); \n" +
							"foreach(var e in Entries) Write(e.Field + ','); \n" +
							"Result=Result.Trim(','); \n" +
							"RelativePath = \"Is\" + RelativePath;"
			});

		Assert.Equal("IsTest.cs", globals.RelativePath);
		Assert.False(globals.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", globals.Result);

		await handler.Handle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "SkipOtherDefinitions = true;"
			});

		Assert.True(globals.SkipOtherDefinitions);
	}
}