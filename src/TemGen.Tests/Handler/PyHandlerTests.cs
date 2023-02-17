using System.Collections.Generic;
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

		var handler = new PySectionHandler(project, new List<Definition>() { definition });
		var result = await handler.Handle(new TemplateSection()
		{
			Handler = TemplateHandler.Python,
			Content = "write(definition.Name + ':') \n" +
						"for val in entries: write(val.Field + ',') \n" +
						"result=result.rstrip(',') \n" +
						"relativePath = \"Is\" + relativePath"
		}, definition, "Test.cs", null);

		Assert.Equal("IsTest.cs", result.RelativePath);
		Assert.False(result.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", result.Content);

		result = await handler.Handle(new TemplateSection()
		{
			Handler = TemplateHandler.Python,
			Content = "skipOtherDefinitions = True"
		}, definition, "IsTest.cs", null);

		Assert.True(result.SkipOtherDefinitions);
	}
}