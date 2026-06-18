using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TemGen.Exceptions;
using TemGen.Models;

namespace TemGen.Services;

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
		var buffer = content.AsSpan();
		Span<char> langSpan = stackalloc char[32];

		while (true)
		{
			var index = buffer.IndexOf("<#");

			if (index == -1)
			{
				sections.Add(new TemplateSection()
				{
					Handler = TemplateHandler.Text,
					Language = null,
					Content = buffer.ToString(),
				});

				break;
			}
			else
			{
				sections.Add(new TemplateSection()
				{
					Handler = TemplateHandler.Text,
					Language = null,
					Content = buffer[..index].ToString(),
				});

				var endIndex = buffer.Slice(index).IndexOf("#>");

				if (endIndex == -1)
				{
					break;
				}

				endIndex += index;

				int langLen = 0;

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

					if (langLen < langSpan.Length)
					{
						langSpan[langLen++] = c;
					}
				}

				var (handler, language) = GetHandlerFromLanguage(langSpan[..langLen]);

				sections.Add(new TemplateSection()
				{
					Handler = handler,
					Language = language,
					Content = buffer[index..endIndex].ToString(),
				});

				buffer = buffer[(endIndex + 2)..];
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
				TemplateHandler.Else => new Exception("Unexpected <#else#> without a matching <#if ...#> block."),
				_ when IsElseIfHandler(terminator.Handler) => new Exception($"Unexpected <#{GetTagName(terminator.Handler, terminator.Language)}#> without a matching <#{GetIfTagName(terminator.Language)} ...#> block."),
				TemplateHandler.End => new Exception("Unexpected <#end#> without a matching control-flow block."),
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
				case TemplateHandler.Else:
				case TemplateHandler.End:
				case var _ when IsElseIfHandler(section.Handler):
					terminator = section;
					return parsedSections;

				case var _ when IsForHandler(section.Handler):
					section.Sections = ParseSections(sections, ref index, out var forTerminator);

					if (forTerminator?.Handler != TemplateHandler.End)
					{
						throw new Exception($"Unclosed <#{GetTagName(section.Handler, section.Language)}#> block. Expected a matching <#end#>.");
					}

					parsedSections.Add(section);
					break;

				case var _ when IsIfHandler(section.Handler):
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
			throw new Exception($"Unclosed <#{GetTagName(section.Handler, section.Language)}#> block. Expected a matching <#end#>.");
		}

		switch (branchTerminator.Handler)
		{
			case TemplateHandler.End:
				return section;

			case TemplateHandler.Else:
				section.ElseSections = ParseSections(sections, ref index, out var elseTerminator);

				if (elseTerminator?.Handler != TemplateHandler.End)
				{
					throw new Exception("Unclosed <#else#> block. Expected a matching <#end#>.");
				}

				return section;

			case var _ when branchTerminator.Handler == TemplateHandler.ElseIf && branchTerminator.Language == section.Language:
				section.ElseSections = [ParseElseIfChain(branchTerminator, sections, ref index)];
				return section;

			default:
				throw new Exception($"Unexpected control-flow token while parsing <#{GetTagName(section.Handler, section.Language)}#> block.");
		}
	}

	private static TemplateSection ParseElseIfChain(TemplateSection token, List<TemplateSection> sections, ref int index)
	{
		var elseIfSection = new TemplateSection
		{
			Handler = token.Handler,
			Language = token.Language,
			Content = token.Content,
		};

		elseIfSection.Sections = ParseSections(sections, ref index, out var branchTerminator);

		if (branchTerminator is null)
		{
			throw new Exception($"Unclosed <#{GetTagName(token.Handler, token.Language)}#> block. Expected a matching <#end#>.");
		}

		switch (branchTerminator.Handler)
		{
			case TemplateHandler.End:
				return elseIfSection;

			case TemplateHandler.Else:
				elseIfSection.ElseSections = ParseSections(sections, ref index, out var elseTerminator);

				if (elseTerminator?.Handler != TemplateHandler.End)
				{
					throw new Exception("Unclosed <#else#> block. Expected a matching <#end#>.");
				}

				return elseIfSection;

			case var _ when branchTerminator.Handler == TemplateHandler.ElseIf && branchTerminator.Language == token.Language:
				elseIfSection.ElseSections = [ParseElseIfChain(branchTerminator, sections, ref index)];
				return elseIfSection;

			default:
				throw new Exception($"Unexpected control-flow token while parsing <#{GetTagName(token.Handler, token.Language)}#> block.");
		}
	}

	private static bool IsForHandler(TemplateHandler handler)
	{
		return handler == TemplateHandler.For;
	}

	private static bool IsIfHandler(TemplateHandler handler)
	{
		return handler == TemplateHandler.If;
	}

	private static bool IsElseIfHandler(TemplateHandler handler)
	{
		return handler == TemplateHandler.ElseIf;
	}

	private static string GetIfTagName(TemplateLanguage? language)
	{
		return $"{GetLanguageTagPrefix(language)}if";
	}

	private static string GetTagName(TemplateHandler handler, TemplateLanguage? language = null)
	{
		return handler switch
		{
			TemplateHandler.Script => GetLanguageTagPrefix(language),
			TemplateHandler.For => $"{GetLanguageTagPrefix(language)}for",
			TemplateHandler.If => $"{GetLanguageTagPrefix(language)}if",
			TemplateHandler.ElseIf => $"{GetLanguageTagPrefix(language)}elseif",
			TemplateHandler.Else => "else",
			TemplateHandler.End => "end",
			TemplateHandler.Reflection => "ref",
			_ => handler.ToString()
		};
	}

	private static string GetLanguageTagPrefix(TemplateLanguage? language)
	{
		return language switch
		{
			TemplateLanguage.CSharp => "cs",
			TemplateLanguage.JavaScript => "js",
			TemplateLanguage.Python => "py",
			TemplateLanguage.Lua => "lua",
			null => string.Empty,
			_ => throw new ArgumentOutOfRangeException(nameof(language), language, "Unsupported template language.")
		};
	}

	private static (TemplateHandler Handler, TemplateLanguage? Language) GetHandlerFromLanguage(ReadOnlySpan<char> language)
	{
		if (language.IsWhiteSpace())
		{
			return (TemplateHandler.Reflection, null);
		}

		language = language.Trim();

		if (language.Equals("cs", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.Script, TemplateLanguage.CSharp);
		if (language.Equals("csfor", StringComparison.OrdinalIgnoreCase) || language.Equals("for", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.For, TemplateLanguage.CSharp);
		if (language.Equals("csif", StringComparison.OrdinalIgnoreCase) || language.Equals("if", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.If, TemplateLanguage.CSharp);
		if (language.Equals("cselseif", StringComparison.OrdinalIgnoreCase) || language.Equals("elseif", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.ElseIf, TemplateLanguage.CSharp);
		if (language.Equals("jsfor", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.For, TemplateLanguage.JavaScript);
		if (language.Equals("jsif", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.If, TemplateLanguage.JavaScript);
		if (language.Equals("jselseif", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.ElseIf, TemplateLanguage.JavaScript);
		if (language.Equals("pyfor", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.For, TemplateLanguage.Python);
		if (language.Equals("pyif", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.If, TemplateLanguage.Python);
		if (language.Equals("pyelseif", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.ElseIf, TemplateLanguage.Python);
		if (language.Equals("luafor", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.For, TemplateLanguage.Lua);
		if (language.Equals("luaif", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.If, TemplateLanguage.Lua);
		if (language.Equals("luaelseif", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.ElseIf, TemplateLanguage.Lua);
		if (language.Equals("else", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.Else, null);
		if (language.Equals("end", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.End, null);
		if (language.Equals("ref", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.Reflection, null);
		if (language.Equals("js", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.Script, TemplateLanguage.JavaScript);
		if (language.Equals("lua", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.Script, TemplateLanguage.Lua);
		if (language.Equals("py", StringComparison.OrdinalIgnoreCase)) return (TemplateHandler.Script, TemplateLanguage.Python);

		return (TemplateHandler.Text, null);
	}

	public static (TemplateHandler Handler, TemplateLanguage? Language, string Content) ReadScript(string resourcesScriptPath)
	{
		resourcesScriptPath = Path.GetFullPath(resourcesScriptPath);

		var extension = Path.GetExtension(resourcesScriptPath).TrimStart('.');
		var (handler, language) = GetHandlerFromLanguage(extension);
		var content = File.ReadAllText(resourcesScriptPath);

		return (handler, language, content);
	}

	public static (TemplateHandler Handler, TemplateLanguage? Language, string Content) ReadScript(string path, string resourcesScriptPath)
	{
		path = Path.GetFullPath(path);
		resourcesScriptPath = Path.IsPathFullyQualified(resourcesScriptPath)
			? resourcesScriptPath
			: Path.Combine(path, resourcesScriptPath);

		return ReadScript(resourcesScriptPath);
	}

	public static List<(TemplateHandler Handler, TemplateLanguage? Language, string Content)> ReadScripts(string scriptsPath)
	{
		var files = Directory.GetFiles(scriptsPath, "*", SearchOption.AllDirectories);
		var templates = files.Select(ReadScript).ToList();
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