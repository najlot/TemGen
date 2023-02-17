using System.Collections.Generic;
using TemGen.Handler;
using Xunit;

namespace TemGen.Tests.Handler;

public class LuaHandlerTests
{
	[Fact]
	public async void LuaMustBeRunCorrect()
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

		var handler = new LuaSectionHandler(project, new List<Definition>() { definition });
		var result = await handler.Handle(new TemplateSection()
		{
			Handler = TemplateHandler.Lua,
			Content = "write(definition.name .. \":\"); \n" +
						"for key, value in ipairs(entries) do write(value.field .. ',') end \n" +
						"result = string.sub(result, 1, -2); \n" +
						"relativePath = \"Is\" .. relativePath;"
		}, definition, "Test.cs", null);

		Assert.Equal("IsTest.cs", result.RelativePath);
		Assert.False(result.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", result.Content);

		result = await handler.Handle(new TemplateSection()
		{
			Handler = TemplateHandler.Lua,
			Content = "skipOtherDefinitions = true"
		}, definition, "IsTest.cs", null);

		Assert.True(result.SkipOtherDefinitions);
	}
}