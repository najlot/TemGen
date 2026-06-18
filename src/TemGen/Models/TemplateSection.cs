using System.Collections.Generic;
using TemGen.Services;

namespace TemGen.Models;

public class TemplateSection
{
	public TemplateHandler Handler { get; set; }
	public TemplateLanguage? Language { get; set; }
	public string Content { get; set; } = string.Empty;
	public List<TemplateSection> Sections { get; set; } = [];
	public List<TemplateSection> ElseSections { get; set; } = [];
}