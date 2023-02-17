using System.Collections.Generic;
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

		var handler = new JintSectionHandler(project, new List<Definition>() { definition });
		var result = await handler.Handle(new TemplateSection()
		{
			Handler = TemplateHandler.JavaScript,
			Content = "write(definition.name + ':'); \n" +
						"for(i in entries) write(entries[i].field + ','); \n" +
						"result=result.substring(0, result.length - 1); \n" +
						"relativePath = \"Is\" + relativePath;"
		}, definition, "Test.cs", null);

		Assert.Equal("IsTest.cs", result.RelativePath);
		Assert.False(result.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", result.Content);

		result = await handler.Handle(new TemplateSection()
		{
			Handler = TemplateHandler.JavaScript,
			Content = "skipOtherDefinitions = true"
		}, definition, "IsTest.cs", null);

		Assert.True(result.SkipOtherDefinitions);
	}
}