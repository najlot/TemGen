using System.Text;

namespace TemGen.Models;

public class HandlingResult
{
	public string RelativePath { get; set; }
	public string Content { get; set; }
	public bool SkipOtherDefinitions { get; set; }
	public bool RepeatForEachDefinitionEntry { get; set; }
	public Encoding Encoding { get; set; }
	public bool AllowOverwrite { get; set; } = true;
}