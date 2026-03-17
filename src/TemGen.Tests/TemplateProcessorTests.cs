using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TemGen.Handler;
using Xunit;

namespace TemGen.Tests;

public class TemplateProcessorTests
{
	[Fact]
	public async Task Handle_must_support_nested_csharp_control_flow_blocks()
	{
		var testId = Path.Combine(".", System.Guid.NewGuid().ToString());
		var templatePath = Path.Combine(testId, "NestedTemplate.txt");
		var content = "<#csfor first in [1, 2]\n#><#csfor second in [3, 4]\n#><#csif first == 1 && second == 3\n#>[HEADER]\n<#end#><#cs Write($\"{first}/{second}\\n\")#><#end#><#end#>";

		Directory.CreateDirectory(testId);
		File.WriteAllText(templatePath, content);

		try
		{
			var template = TemplatesReader.ReadTemplates(testId).Single();
			var definition = new Definition
			{
				Name = "TodoItem",
				NameLow = "todoItem",
				Entries = []
			};
			var project = new Project
			{
				Namespace = "Todo"
			};
			var processor = new TemplateProcessor(
			[
				new TextSectionHandler(),
				new CsSectionHandler([]),
				new ReflectionSectionHandler()
			],
			project,
			[definition]);

			var result = await processor.Handle(template, definition, null);

			Assert.Equal("[HEADER]\n1/3\n1/4\n2/3\n2/4\n", result.Content.Replace("\r\n", "\n"));
		}
		finally
		{
			Directory.Delete(testId, true);
		}
	}

	[Fact]
	public async Task Handle_must_support_if_else_blocks_with_scoped_loop_variables()
	{
		var testId = Path.Combine(".", System.Guid.NewGuid().ToString());
		var templatePath = Path.Combine(testId, "ConditionalTemplate.txt");
		var content = "<#csfor entry in Definition.Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2)\n#><#csif entry.EntryType.ToLower() == \"string\"\n#>STRING:<#cs Write(Definition.NameLow + \".\" + entry.Field)#>\n<#else\n#>OTHER:<#cs Write(Definition.NameLow + \".\" + entry.Field)#>\n<#end#><#end#><#csif Entries.Where(e => e.IsArray).Any()\n#>SECTION\n<#end#>";

		Directory.CreateDirectory(testId);
		File.WriteAllText(templatePath, content);

		try
		{
			var template = TemplatesReader.ReadTemplates(testId).Single();
			var definition = new Definition
			{
				Name = "TodoItem",
				NameLow = "todoItem",
				Entries =
				[
					new DefinitionEntry { Field = "Title", EntryType = "string" },
					new DefinitionEntry { Field = "Priority", EntryType = "int" },
					new DefinitionEntry { Field = "Checklist", EntryType = "ChecklistTask", IsArray = true }
				]
			};
			var project = new Project
			{
				Namespace = "Todo"
			};
			var processor = new TemplateProcessor(
			[
				new TextSectionHandler(),
				new CsSectionHandler([]),
				new ReflectionSectionHandler()
			],
			project,
			[definition]);

			var result = await processor.Handle(template, definition, null);

			Assert.Equal("STRING:todoItem.Title\nOTHER:todoItem.Priority\nSECTION\n", result.Content.Replace("\r\n", "\n"));
		}
		finally
		{
			Directory.Delete(testId, true);
		}
	}

	[Fact]
	public async Task Handle_must_support_else_if_chains()
	{
		var testId = Path.Combine(".", System.Guid.NewGuid().ToString());
		var templatePath = Path.Combine(testId, "ElseIfTemplate.txt");
		var content = "<#csfor value in [1, 2, 3, 4, 5]\n#><#csif value == 1#>A<#cselseif value == 2#>B<#csElseIf value == 3#>C<#cselseif value == 4#>D<#else#>E<#end#><#end#>";

		Directory.CreateDirectory(testId);
		File.WriteAllText(templatePath, content);

		try
		{
			var template = TemplatesReader.ReadTemplates(testId).Single();
			var definition = new Definition
			{
				Name = "TodoItem",
				NameLow = "todoItem",
				Entries = []
			};
			var project = new Project
			{
				Namespace = "Todo"
			};
			var processor = new TemplateProcessor(
			[
				new TextSectionHandler(),
				new CsSectionHandler([]),
				new ReflectionSectionHandler()
			],
			project,
			[definition]);

			var result = await processor.Handle(template, definition, null);

			Assert.Equal("ABCDE", result.Content.Replace("\r\n", "\n"));
		}
		finally
		{
			Directory.Delete(testId, true);
		}
	}

	[Fact]
	public async Task Handle_must_ignore_whitespace_only_reflection_sections()
	{
		var testId = Path.Combine(".", System.Guid.NewGuid().ToString());
		var templatePath = Path.Combine(testId, "EmptyReflectionTemplate.txt");
		var content = "A<#   #>B";

		Directory.CreateDirectory(testId);
		File.WriteAllText(templatePath, content);

		try
		{
			var template = TemplatesReader.ReadTemplates(testId).Single();
			var definition = new Definition
			{
				Name = "TodoItem",
				NameLow = "todoItem",
				Entries = []
			};
			var project = new Project
			{
				Namespace = "Todo"
			};
			var processor = new TemplateProcessor(
			[
				new TextSectionHandler(),
				new CsSectionHandler([]),
				new ReflectionSectionHandler()
			],
			project,
			[definition]);

			var result = await processor.Handle(template, definition, null);

			Assert.Equal("AB", result.Content);
		}
		finally
		{
			Directory.Delete(testId, true);
		}
	}
}