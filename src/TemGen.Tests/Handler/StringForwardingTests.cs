using System.Threading.Tasks;
using TemGen.Handler;
using Xunit;

namespace TemGen.Tests.Handler;

public class StringForwardingTests
{
	private static Globals CreateGlobals() => new()
	{
		Definitions = [],
		Definition = new Definition() { Entries = [] },
		Project = new Project(),
		RelativePath = "",
	};

	// === C# as source ===

	[Fact]
	public async Task ForwardCs2Js()
	{
		var globals = CreateGlobals();

		var csHandler = new CsSectionHandler([]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "SetVariable(\"Greeting\", \"Hello\");"
			});

		var jsHandler = new JintSectionHandler([]);
		await jsHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "write(getVariable(\"Greeting\"));"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardCs2Py()
	{
		var globals = CreateGlobals();

		var csHandler = new CsSectionHandler([]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "SetVariable(\"Greeting\", \"Hello\");"
			});

		var pyHandler = new PySectionHandler([]);
		await pyHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Python,
				Content = "write(get_variable(\"Greeting\"))"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardCs2Lua()
	{
		var globals = CreateGlobals();

		var csHandler = new CsSectionHandler([]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "SetVariable(\"Greeting\", \"Hello\");"
			});

		var luaHandler = new LuaSectionHandler([]);
		await luaHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Lua,
				Content = "write(get_variable(\"Greeting\"))"
			});

		Assert.Equal("Hello", globals.Result);
	}

	// === JavaScript as source ===

	[Fact]
	public async Task ForwardJs2Cs()
	{
		var globals = CreateGlobals();

		var jsHandler = new JintSectionHandler([]);
		await jsHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "setVariable(\"Greeting\", \"Hello\");"
			});

		var csHandler = new CsSectionHandler([]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "Write((string)GetVariable(\"Greeting\"));"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardJs2Py()
	{
		var globals = CreateGlobals();

		var jsHandler = new JintSectionHandler([]);
		await jsHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "setVariable(\"Greeting\", \"Hello\");"
			});

		var pyHandler = new PySectionHandler([]);
		await pyHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Python,
				Content = "write(get_variable(\"Greeting\"))"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardJs2Lua()
	{
		var globals = CreateGlobals();

		var jsHandler = new JintSectionHandler([]);
		await jsHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "setVariable(\"Greeting\", \"Hello\");"
			});

		var luaHandler = new LuaSectionHandler([]);
		await luaHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Lua,
				Content = "write(get_variable(\"Greeting\"))"
			});

		Assert.Equal("Hello", globals.Result);
	}

	// === Python as source ===

	[Fact]
	public async Task ForwardPy2Cs()
	{
		var globals = CreateGlobals();

		var pyHandler = new PySectionHandler([]);
		await pyHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Python,
				Content = "set_variable(\"Greeting\", \"Hello\")"
			});

		var csHandler = new CsSectionHandler([]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "Write((string)GetVariable(\"Greeting\"));"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardPy2Js()
	{
		var globals = CreateGlobals();

		var pyHandler = new PySectionHandler([]);
		await pyHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Python,
				Content = "set_variable(\"Greeting\", \"Hello\")"
			});

		var jsHandler = new JintSectionHandler([]);
		await jsHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "write(getVariable(\"Greeting\"));"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardPy2Lua()
	{
		var globals = CreateGlobals();

		var pyHandler = new PySectionHandler([]);
		await pyHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Python,
				Content = "set_variable(\"Greeting\", \"Hello\")"
			});

		var luaHandler = new LuaSectionHandler([]);
		await luaHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Lua,
				Content = "write(get_variable(\"Greeting\"))"
			});

		Assert.Equal("Hello", globals.Result);
	}

	// === Lua as source ===

	[Fact]
	public async Task ForwardLua2Cs()
	{
		var globals = CreateGlobals();

		var luaHandler = new LuaSectionHandler([]);
		await luaHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Lua,
				Content = "set_variable(\"Greeting\", \"Hello\")"
			});

		var csHandler = new CsSectionHandler([]);
		await csHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.CSharp,
				Content = "Write((string)GetVariable(\"Greeting\"));"
			});

		Assert.Equal("Hello", globals.Result);
	}

	[Fact]
	public async Task ForwardLua2Js()
	{
		var globals = CreateGlobals();

		var luaHandler = new LuaSectionHandler([]);
		await luaHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Lua,
				Content = "set_variable(\"Greeting\", \"Hello\")"
			});

		var jsHandler = new JintSectionHandler([]);
		await jsHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.JavaScript,
				Content = "write(getVariable(\"Greeting\"));"
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
				Handler = TemplateHandler.Lua,
				Content = "set_variable(\"Greeting\", \"Hello\")"
			});

		var pyHandler = new PySectionHandler([]);
		await pyHandler.TryHandle(
			globals,
			new TemplateSection()
			{
				Handler = TemplateHandler.Python,
				Content = "write(get_variable(\"Greeting\"))"
			});

		Assert.Equal("Hello", globals.Result);
	}
}
