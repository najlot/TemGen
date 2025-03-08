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

		var subScript = "void w(object obj)\r\n{\r\nWrite(obj);\r\n}";

		var handler = new CsSectionHandler([subScript]);
		await handler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "w(Definition.Name + ':'); \n" +
							"foreach(var e in Entries) Write(e.Field + ','); \n" +
							"Result=Result.Trim(','); \n" +
							"RelativePath = \"Is\" + RelativePath;"
			});

		Assert.Equal("IsTest.cs", globals.RelativePath);
		Assert.False(globals.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", globals.Result);

		await handler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "SkipOtherDefinitions = true;"
			});

		Assert.True(globals.SkipOtherDefinitions);
	}
}