using System.Collections.Generic;

namespace TemGen;

public class Definition
{
	public string Name { get; set; }
	public string NameLow { get; set; }
	public bool IsOwnedType { get; set; }
	public bool IsEnumeration { get; set; }
	public bool IsArray { get; set; }
	public List<DefinitionEntry> Entries { get; set; }
}