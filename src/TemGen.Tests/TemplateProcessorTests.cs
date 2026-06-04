using System.IO;
using System.Linq;
using System.Reflection;
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
	public async Task Handle_must_stop_processing_remaining_sections_when_skip_remaining_is_returned_from_csharp()
	{
		var testId = Path.Combine(".", System.Guid.NewGuid().ToString());
		var templatePath = Path.Combine(testId, "SkipRemainingReturnTemplate.txt");
		var content = "START<#cs Write(\"A\"); return SkipRemaining();#>END";

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

			Assert.Equal("STARTA", result.Content.Replace("\r\n", "\n"));
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

	[Fact]
	public async Task Handle_must_expose_empty_previous_content_by_default()
	{
		var testId = Path.Combine(".", System.Guid.NewGuid().ToString());
		var templatePath = Path.Combine(testId, "PreviousContentDefaultTemplate.txt");
		var content = "<#cs Write(string.IsNullOrEmpty(PreviousContent) ? \"EMPTY\" : PreviousContent)#>";

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

			Assert.Equal("EMPTY", result.Content);
		}
		finally
		{
			Directory.Delete(testId, true);
		}
	}

	[Fact]
	public async Task Handle_must_expose_previous_content_when_provided()
	{
		var testId = Path.Combine(".", System.Guid.NewGuid().ToString());
		var templatePath = Path.Combine(testId, "PreviousContentTemplate.txt");
		var content = "<#cs Write(PreviousContent)#>";

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

			var result = await processor.Handle(template, definition, null, "BASE CONTENT");

			Assert.Equal("BASE CONTENT", result.Content);
		}
		finally
		{
			Directory.Delete(testId, true);
		}
	}

	[Fact]
	public async Task HandleLayer_must_fall_back_to_previous_output_path_content_when_definition_context_differs()
	{
		var testId = Path.Combine(".", System.Guid.NewGuid().ToString());
		var baseTemplatePath = Path.Combine(testId, "Base");
		var overrideTemplatePath = Path.Combine(testId, "Override");
		var baseReadmePath = Path.Combine(baseTemplatePath, "README.md");
		var overrideReadmePath = Path.Combine(overrideTemplatePath, "README.md");

		Directory.CreateDirectory(baseTemplatePath);
		Directory.CreateDirectory(overrideTemplatePath);
		File.WriteAllText(baseReadmePath, "Base layer\n<#cs SkipOtherDefinitions = true;#>");
		File.WriteAllText(overrideReadmePath, "<#cs Write(PreviousContent);#>Override layer");

		try
		{
			var baseTemplate = TemplatesReader.ReadTemplates(baseTemplatePath).Single();
			var overrideTemplate = TemplatesReader.ReadTemplates(overrideTemplatePath).Single();
			var definitions =
				new[]
				{
					new Definition { Name = "Note", NameLow = "note", Entries = [] },
					new Definition { Name = "TodoItem", NameLow = "todoItem", Entries = [] }
				}.ToList();
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
			definitions);

			var handleLayer = typeof(TemplateProcessor).GetMethod("HandleLayer", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.NotNull(handleLayer);

			var baseLayerTask = (Task)handleLayer.Invoke(processor, [baseTemplate, null]);
			await baseLayerTask;
			var baseLayerResult = baseLayerTask.GetType().GetProperty("Result")?.GetValue(baseLayerTask);
			Assert.NotNull(baseLayerResult);

			var layerContents = (System.Collections.IDictionary)baseLayerResult.GetType().GetField("Item2")?.GetValue(baseLayerResult);
			Assert.NotNull(layerContents);

			var previousLayerContents = ((System.Collections.IEnumerable)layerContents)
				.Cast<object>()
				.ToDictionary(
					entry => (string)entry.GetType().GetProperty("Key")?.GetValue(entry),
					entry => (string)entry.GetType().GetProperty("Value")?.GetValue(entry));

			var overrideLayerTask = (Task)handleLayer.Invoke(processor, [overrideTemplate, previousLayerContents]);
			await overrideLayerTask;
			var overrideLayerResult = overrideLayerTask.GetType().GetProperty("Result")?.GetValue(overrideLayerTask);
			Assert.NotNull(overrideLayerResult);

			var files = (System.Collections.IDictionary)overrideLayerResult.GetType().GetField("Item1")?.GetValue(overrideLayerResult);
			Assert.NotNull(files);

			var readmeEntry = ((System.Collections.IEnumerable)files)
				.Cast<object>()
				.Single(entry => (string)entry.GetType().GetProperty("Key")?.GetValue(entry) == "README.md");
			var readmeValue = readmeEntry.GetType().GetProperty("Value")?.GetValue(readmeEntry);
			var content = (string)readmeValue.GetType().GetField("Item2")?.GetValue(readmeValue);

			Assert.Equal("Base layer\nOverride layer", content.Replace("\r\n", "\n"));
		}
		finally
		{
			Directory.Delete(testId, true);
		}
	}
}