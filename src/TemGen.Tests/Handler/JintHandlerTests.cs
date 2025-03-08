using System.Threading.Tasks;
using TemGen.Handler;
using Xunit;

namespace TemGen.Tests.Handler;

public class JintHandlerTests
{
	[Fact]
	public async Task JsMustBeRunCorrect()
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

		var subScript = "function w(param)\r\n{\r\nwrite(param);\r\n}";

		var handler = new JintSectionHandler([subScript]);
		await handler.TryHandle(globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "w(definition.name + ':'); \n" +
							"for(i in entries) write(entries[i].field + ','); \n" +
							"result = getResult(); \n" +
							"result = result.substring(0, result.length - 1); \n" +
							"setResult(result); \n" +
							"relativePath = \"Is\" + relativePath;"
			});

		Assert.Equal("IsTest.cs", globals.RelativePath);
		Assert.False(globals.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", globals.Result);

		await handler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "skipOtherDefinitions = true"
			});

		Assert.True(globals.SkipOtherDefinitions);
	}
}