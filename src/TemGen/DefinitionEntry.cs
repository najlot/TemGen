namespace TemGen;

public class DefinitionEntry
{
	public string EntryType { get; set; }
	public string EntryTypeLow { get; set; }
	public string Field { get; set; }
	public string FieldLow { get; set; }
	public bool IsOwnedType { get; set; }
	public bool IsKey { get; set; }
	public bool IsArray { get; set; }
	public bool IsReference { get; set; }
	public bool IsEnumeration { get; set; }
	public bool IsNullable { get; set; }
	public string ReferenceType { get; set; }
	public string ReferenceTypeLow { get; set; }
}