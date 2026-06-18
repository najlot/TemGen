using System.Threading.Tasks;
using TemGen.Handler;
using TemGen.Models;
using TemGen.Services;
using Xunit;

namespace TemGen.Tests.Handler;

public class FunctionForwardingTests
{
	private static Globals CreateGlobals() => new()
	{
		Definitions = [],
		Definition = new Definition() { Entries = [] },
		Project = new Project(),
		RelativePath = "",
	};

	[Fact]
	public async Task ForwardCs2Js()
	{
		var globals = CreateGlobals();

		var csHandler = new CsSectionHandler(["void WriteHello(){Write(\"Hello\");}"]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.CSharp,
				Content = "SetVariable(\"WriteHello\", () => WriteHello());"
			});

		var jsHandler = new JintSectionHandler([]);
		await jsHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.JavaScript,
				Content = "getVariable(\"WriteHello\")();"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardCs2Py()
	{
		var globals = CreateGlobals();

		var csHandler = new CsSectionHandler(["void WriteHello(){Write(\"Hello\");}"]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.CSharp,
				Content = "SetVariable(\"WriteHello\", () => WriteHello());"
			});

		var pyHandler = new PySectionHandler([]);
		await pyHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.Python,
				Content = "get_variable(\"WriteHello\")()"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardCs2Lua()
	{
		var globals = CreateGlobals();

		var csHandler = new CsSectionHandler(["void WriteHello(){Write(\"Hello\");}"]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.CSharp,
				Content = "SetVariable(\"WriteHello\", () => WriteHello());"
			});

		var luaHandler = new LuaSectionHandler([]);
		await luaHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.Lua,
				Content = "get_variable(\"WriteHello\")()"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardLua2Py()
	{
		var globals = CreateGlobals();

		var luaHandler = new LuaSectionHandler([]);
		await luaHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.Lua,
				Content = "set_variable(\"WriteHello\", function() write(\"Hello\") end)"
			});

		var pyHandler = new PySectionHandler([]);
		await pyHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Script,
				Language = TemplateLanguage.Python,
				Content = "get_variable(\"WriteHello\")()"
			});

		Assert.Equal("Hello", globals.Result);
	}
}
