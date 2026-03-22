using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TemGen;

public static class TemplatesReader
{
	private static List<TemplateSection> GetSections(string content)
	{
		var sections = ReadSections(content);
		return BuildNestedSections(sections);
	}

	private static List<TemplateSection> ReadSections(string content)
	{
		var sections = new List<TemplateSection>();
		Span<char> buffer = content.ToCharArray();

		while (true)
		{
			var index = buffer.IndexOf("<#");

			if (index == -1)
			{
				sections.Add(new TemplateSection()
				{
					Handler = TemplateHandler.Text,
					Content = new string(buffer),
				});

				break;
			}
			else
			{
				sections.Add(new TemplateSection()
				{
					Handler = TemplateHandler.Text,
					Content = new string(buffer.Slice(0, index)),
				});

				var language = "";
				var endIndex = buffer.Slice(index).IndexOf("#>");

				if (endIndex == -1)
				{
					break;
				}

				endIndex += index;

				for (index += 2; index < endIndex; index++)
				{
					char c = buffer[index];

					if (c == '\r')
					{
						continue;
					}

					if (c == ' ' || c == '\n')
					{
						index++;
						break;
					}

					language += c;
				}

				TemplateHandler handler = GetHandlerFromLanguage(language);

				sections.Add(new TemplateSection()
				{
					Handler = handler,
					Content = new string(buffer[index..endIndex]),
				});

				buffer = buffer.Slice(endIndex + 2);
			}
		}

		return sections;
	}

	private static List<TemplateSection> BuildNestedSections(List<TemplateSection> sections)
	{
		var index = 0;
		var rootSections = ParseSections(sections, ref index, out var terminator);

		if (terminator is not null)
		{
			throw terminator.Handler switch
			{
				TemplateHandler.CSharpElse => new Exception("Unexpected <#else#> without a matching <#csif ...#> block."),
				TemplateHandler.CSharpElseIf => new Exception("Unexpected <#cselseif#> without a matching <#csif ...#> block."),
				TemplateHandler.CSharpEnd => new Exception("Unexpected <#end#> without a matching control-flow block."),
				_ => new Exception("Unexpected control-flow token while parsing template sections.")
			};
		}

		return rootSections;
	}

	private static List<TemplateSection> ParseSections(List<TemplateSection> sections, ref int index, out TemplateSection terminator)
	{
		var parsedSections = new List<TemplateSection>();
		terminator = null;

		while (index < sections.Count)
		{
			var section = sections[index++];

			switch (section.Handler)
			{
				case TemplateHandler.CSharpElse:
				case TemplateHandler.CSharpElseIf:
				case TemplateHandler.CSharpEnd:
					terminator = section;
					return parsedSections;

				case TemplateHandler.CSharpFor:
					section.Sections = ParseSections(sections, ref index, out var forTerminator);

					if (forTerminator?.Handler != TemplateHandler.CSharpEnd)
					{
						throw new Exception("Unclosed <#csfor#> block. Expected a matching <#end#>.");
					}

					parsedSections.Add(section);
					break;

				case TemplateHandler.CSharpIf:
					parsedSections.Add(ParseIfSection(section, sections, ref index));
					break;

				default:
					parsedSections.Add(section);
					break;
			}
		}

		return parsedSections;
	}

	private static TemplateSection ParseIfSection(TemplateSection section, List<TemplateSection> sections, ref int index)
	{
		section.Sections = ParseSections(sections, ref index, out var branchTerminator);

		if (branchTerminator is null)
		{
			throw new Exception("Unclosed <#csif#> block. Expected a matching <#end#>.");
		}

		switch (branchTerminator.Handler)
		{
			case TemplateHandler.CSharpEnd:
				return section;

			case TemplateHandler.CSharpElse:
				section.ElseSections = ParseSections(sections, ref index, out var elseTerminator);

				if (elseTerminator?.Handler != TemplateHandler.CSharpEnd)
				{
					throw new Exception("Unclosed <#else#> block. Expected a matching <#end#>.");
				}

				return section;

			case TemplateHandler.CSharpElseIf:
				section.ElseSections = [ParseElseIfChain(branchTerminator, sections, ref index)];
				return section;

			default:
				throw new Exception("Unexpected control-flow token while parsing <#csif#> block.");
		}
	}

	private static TemplateSection ParseElseIfChain(TemplateSection token, List<TemplateSection> sections, ref int index)
	{
		var elseIfSection = new TemplateSection
		{
			Handler = TemplateHandler.CSharpElseIf,
			Content = token.Content,
		};

		elseIfSection.Sections = ParseSections(sections, ref index, out var branchTerminator);

		if (branchTerminator is null)
		{
			throw new Exception("Unclosed <#cselseif#> block. Expected a matching <#end#>.");
		}

		switch (branchTerminator.Handler)
		{
			case TemplateHandler.CSharpEnd:
				return elseIfSection;

			case TemplateHandler.CSharpElse:
				elseIfSection.ElseSections = ParseSections(sections, ref index, out var elseTerminator);

				if (elseTerminator?.Handler != TemplateHandler.CSharpEnd)
				{
					throw new Exception("Unclosed <#else#> block. Expected a matching <#end#>.");
				}

				return elseIfSection;

			case TemplateHandler.CSharpElseIf:
				elseIfSection.ElseSections = [ParseElseIfChain(branchTerminator, sections, ref index)];
				return elseIfSection;

			default:
				throw new Exception("Unexpected control-flow token while parsing <#cselseif#> block.");
		}
	}

	private static TemplateHandler GetHandlerFromLanguage(string language)
	{
		if (string.IsNullOrWhiteSpace(language))
		{
			// Default value when the input is invalid
			return TemplateHandler.Reflection;
		}

		language = language.Trim().ToLowerInvariant();

		return language switch
		{
			"cs" => TemplateHandler.CSharp, // C# template handler
			"csfor" or "for" => TemplateHandler.CSharpFor,
			"csif" or "if" => TemplateHandler.CSharpIf,
			"cselseif" or "elseif" => TemplateHandler.CSharpElseIf,
			"else" => TemplateHandler.CSharpElse,
			"end" => TemplateHandler.CSharpEnd,
			"ref" => TemplateHandler.Reflection, // Reflection template handler
			"js" => TemplateHandler.JavaScript, // JavaScript template handler
			"lua" => TemplateHandler.Lua, // Lua template handler
			"py" => TemplateHandler.Python, // Python template handler
			_ => TemplateHandler.Text, // Default value when the language is not recognized
		};
	}

	public static Template ReadScript(string path, string resourcesScriptPath)
	{
		path = Path.GetFullPath(path);
		resourcesScriptPath = Path.Combine(path, resourcesScriptPath);

		var extension = Path.GetExtension(resourcesScriptPath).TrimStart('.');
		var handler = GetHandlerFromLanguage(extension);
		var content = File.ReadAllText(resourcesScriptPath);

		return new Template()
		{
			RelativePath = Path.GetRelativePath(path, resourcesScriptPath),
			Sections = [new() { Content = content, Handler = handler }]
		};
	}

	public static List<Template> ReadScripts(string scriptsPath)
	{
		var files = Directory.GetFiles(scriptsPath, "*", SearchOption.AllDirectories);
		var templates = files.Select(file => ReadScript(scriptsPath, file)).ToList();
		return templates;
	}

	public static List<Template> ReadTemplates(string templatesPath)
	{
		var files = Directory.GetFiles(templatesPath, "*", SearchOption.AllDirectories);
		var templates = new List<Template>(files.Length);
		var errors = new List<Exception>();

		foreach (var file in files)
		{
			try
			{
				using var reader = new StreamReader(file, Encoding.Default, true);
				var content = reader.ReadToEnd();
				var sections = GetSections(content);
				var relativePath = Path.GetRelativePath(templatesPath, file);

				templates.Add(new Template()
				{
					Encoding = reader.CurrentEncoding,
					RelativePath = relativePath,
					Sections = sections
				});
			}
			catch (Exception ex) when (ex is not TemplateReadException)
			{
				var relativePath = Path.GetRelativePath(templatesPath, file);
				errors.Add(new TemplateReadException(relativePath, ex.Message, ex));
			}
		}

		if (errors.Count > 0)
		{
			throw new AggregateException($"Failed to read {errors.Count} template(s) from '{templatesPath}'.", errors);
		}

		return templates;
	}
}