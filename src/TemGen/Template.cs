using System.Collections.Generic;
using System.Text;

namespace TemGen;

public class Template
{
	public string RelativePath { get; set; }
	public List<TemplateSection> Sections { get; set; }
	public Encoding Encoding { get; set; }
}