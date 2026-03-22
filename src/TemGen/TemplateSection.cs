using System.Collections.Generic;

namespace TemGen;

public class TemplateSection
{
	public TemplateHandler Handler { get; set; }
	public string Content { get; set; } = string.Empty;
	public List<TemplateSection> Sections { get; set; } = [];
	public List<TemplateSection> ElseSections { get; set; } = [];
}