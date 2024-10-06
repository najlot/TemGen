using System.Threading.Tasks;
using TemGen.Handler;
using Xunit;

namespace TemGen.Tests.Handler;

public class PyHandlerTests
{
	[Fact]
	public async Task PyMustBeRunCorrect()
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

		var handler = new PySectionHandler();
		await handler.Handle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Python,
				Content = "write(definition.Name + ':') \n" +
							"for val in entries: write(val.Field + ',') \n" +
							"set_result(get_result().rstrip(',')) \n" +
							"relative_path = \"Is\" + relative_path"
			});

		Assert.Equal("IsTest.cs", globals.RelativePath);
		Assert.False(globals.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", globals.Result);

		await handler.Handle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Python,
				Content = "skip_other_definitions = True"
			});

		Assert.True(globals.SkipOtherDefinitions);
	}
}