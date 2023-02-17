using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

	private static IEnumerable<TemplateSection> GetSections_old(string content)
	{
		while (true)
		{
			var index = content.IndexOf("<#");

			if (index == -1)
			{
				yield return new TemplateSection()
				{
					Handler = TemplateHandler.Text,
					Content = content,
				};

				break;
			}
			else
			{
				yield return new TemplateSection()
				{
					Handler = TemplateHandler.Text,
					Content = content.Substring(0, index),
				};

				var language = "";
				var endIndex = content.IndexOf("#>", index);

				if (endIndex == -1)
				{
					break;
				}

				for (index += 2; index < endIndex; index++)
				{
					char c = content[index];

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

				yield return new TemplateSection()
				{
					Handler = handler,
					Content = content[index..endIndex],
				};

				content = content.Substring(endIndex + 2);
			}
		}
	}

	private static TemplateHandler GetHandlerFromLanguage(string language)
	{
		var handler = TemplateHandler.Text;

		switch (language.Trim().ToLower())
		{
			case "cs":
				handler = TemplateHandler.CSharp;
				break;

			case "ref":
				handler = TemplateHandler.Reflection;
				break;

			case "js":
				handler = TemplateHandler.JavaScript;
				break;

			case "lua":
				handler = TemplateHandler.Lua;
				break;

			case "py":
				handler = TemplateHandler.Python;
				break;
		}

		return handler;
	}

	public static Template ReadResourceScript(string resourcesScriptPath)
	{
		var extension = Path.GetExtension(resourcesScriptPath).TrimStart('.');
		var handler = GetHandlerFromLanguage(extension);

		var content = File.ReadAllText(resourcesScriptPath);
		var sections = new List<TemplateSection>()
	{
		new TemplateSection()
		{
			Content = content,
			Handler = handler
		}
	};

		return new Template()
		{
			RelativePath = Path.GetRelativePath(resourcesScriptPath, resourcesScriptPath),
			Sections = sections
		};
	}

	/// <summary>
	/// Get File's Encoding
	/// </summary>
	/// <param name="filename">The path to the file</param>
	/// <returns></returns>
	private static Encoding GetEncoding(string filename)
	{
		// This is a direct quote from MSDN:
		// The CurrentEncoding value can be different after the first
		// call to any Read method of StreamReader, since encoding
		// autodetection is not done until the first call to a Read method.

		using (var reader = new StreamReader(filename, Encoding.Default, true))
		{
			if (reader.Peek() >= 0) // you need this!
				reader.Read();

			return reader.CurrentEncoding;
		}
	}

	public static List<Template> ReadTemplates(string templatesPath)
	{
		var files = Directory.GetFiles(templatesPath, "*", SearchOption.AllDirectories);
		var templates = new List<Template>(files.Length);

		foreach (var file in files)
		{
			var content = File.ReadAllText(file);
			var sections = GetSections(content).ToList();

			templates.Add(new Template()
			{
				Encoding = GetEncoding(file),
				RelativePath = Path.GetRelativePath(templatesPath, file),
				Sections = sections
			});
		}

		return templates;
	}
}