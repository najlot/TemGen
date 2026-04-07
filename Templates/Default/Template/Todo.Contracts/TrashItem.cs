using System;

namespace <# Project.Namespace#>.Contracts;

public enum ItemType
{
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name)
#>	<# definition.Name#>,
<#end#>}

public sealed class TrashItem
{
	public Guid Id { get; set; }
	public ItemType Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public DateTime? DeletedAt { get; set; }
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>