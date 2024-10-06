using System.Collections.Generic;
using System.Diagnostics;
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
			Entries = new List<DefinitionEntry>()
		{
			new DefinitionEntry()
			{
				Field = "Entry_1"
			},
			new DefinitionEntry()
			{
				Field = "Entry_2"
			},
		}
		};

		var globals = new Globals()
		{
			RelativePath = "Test.cs",
			Definition = definition,
			Definitions = new List<Definition>() { definition },
			DefinitionEntry = null,
			Entries = definition.Entries,
			SkipOtherDefinitions = false,
			Project = project,
			RepeatForEachDefinitionEntry = false
		};

		var handler = new JintSectionHandler();
		await handler.Handle(globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "write(definition.name + ':'); \n" +
							"for(i in entries) write(entries[i].field + ','); \n" +
							"result = getResult(); \n" +
							"result = result.substring(0, result.length - 1); \n" +
							"setResult(result); \n" +
							"relativePath = \"Is\" + relativePath;"
			});

		Assert.Equal("IsTest.cs", globals.RelativePath);
		Assert.False(globals.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", globals.Result);

		await handler.Handle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "skipOtherDefinitions = true"
			});

		Assert.True(globals.SkipOtherDefinitions);
	}
}