using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Community.CsharpSqlite.Sqlite3;

namespace TemGen;

public static class TemplatesReader
{
	private static List<TemplateSection> GetSections(string contentoz)
	{
		var sections = new List<TemplateSection>();
		Span<char> buffer = contentoz.ToCharArray();

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

	private static TemplateHandler GetHandlerFromLanguage(string language)
	{
		if (string.IsNullOrWhiteSpace(language))
		{
			// Default value when the input is invalid
			return TemplateHandler.Text;
		}

		language = language.Trim().ToLowerInvariant();

		return language switch
		{
			"cs" => TemplateHandler.CSharp, // C# template handler
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

		foreach (var file in files)
		{
			using var reader = new StreamReader(file, Encoding.Default, true);
			var content = reader.ReadToEnd();
			var sections = GetSections(content);

			templates.Add(new Template()
			{
				Encoding = reader.CurrentEncoding,
				RelativePath = Path.GetRelativePath(templatesPath, file),
				Sections = sections
			});
		}

		return templates;
	}
}