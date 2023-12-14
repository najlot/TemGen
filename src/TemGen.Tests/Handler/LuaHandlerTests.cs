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

		var handler = new LuaSectionHandler();
		await handler.Handle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Lua,
				Content = "write(definition.name .. \":\"); \n" +
							"for key, value in ipairs(entries) do write(value.field .. ',') end \n" +
							"set_result(string.sub(get_result(), 1, -2)); \n" +
							"relative_path = \"Is\" .. relative_path;"
			});

		Assert.Equal("IsTest.cs", globals.RelativePath);
		Assert.False(globals.SkipOtherDefinitions);
		Assert.Equal("Test:Entry_1,Entry_2", globals.Result);

		await handler.Handle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Lua,
				Content = "skip_other_definitions = true"
			});

		Assert.True(globals.SkipOtherDefinitions);
	}
}