using System.Threading.Tasks;
using TemGen.Handler;
using TemGen.Services;
using Xunit;

namespace TemGen.Tests.Handler;

public class LuaHandlerTests
{
	[Fact]
	public async Task LuaMustBeRunCorrect()
	{
		var project = new Project()
		{
			Namespace = "TestNamespace",
		};
		project.SetSetting("UserSecretsId", "aspnet-TestNamespace.Blazor-lua");

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

		var subScript = "function w(param)\r\nwrite(param)\r\nend";

		var handler = new LuaSectionHandler([subScript]);
		await handler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.Lua,
				Content = "w(project.settings['UserSecretsId'] .. '|'); \n" +
						"w(definition.name .. \":\"); \n" +
							"for key, value in ipairs(entries) do write(value.field .. ',') end \n" +
							"set_result(string.sub(get_result(), 1, -2)); \n" +
							"relative_path = \"Is\" .. relative_path; \n" +
							"allow_overwrite = false;"
			});

		Assert.Equal("IsTest.cs", globals.RelativePath);
		Assert.False(globals.AllowOverwrite);
		Assert.False(globals.SkipOtherDefinitions);
		Assert.Equal("aspnet-TestNamespace.Blazor-lua|Test:Entry_1,Entry_2", globals.Result);

		await handler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.Lua,
				Content = "skip_other_definitions = true"
			});

		Assert.True(globals.SkipOtherDefinitions);

		await handler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.Lua,
				Content = "skip_remaining()"
			});

		Assert.True(globals.SkipRemainingRequested);
	}
}