using System.Collections.Generic;
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

		var handler = new CsSectionHandler(project, new List<Definition>() { definition });
		var result = await handler.Handle(new TemplateSection()
		{
			Handler = TemplateHandler.CSharp,
			Content = "Write(Definition.Name + ':'); \n" +
						"foreach(var e in Entries) Write(e.Field + ','); \n" +
						"Result=Result.Trim(','); \n" +
						"RelativePath = \"Is\" + RelativePath;"
		}, definition, "Test.cs", null);

		Assert.Equal("IsTest.cs", result.RelativePath);
		Assert.False(result.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", result.Content);

		result = await handler.Handle(new TemplateSection()
		{
			Handler = TemplateHandler.CSharp,
			Content = "SkipOtherDefinitions = true;"
		}, definition, "IsTest.cs", null);

		Assert.True(result.SkipOtherDefinitions);
	}
}