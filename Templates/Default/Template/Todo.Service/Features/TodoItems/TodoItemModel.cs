using MongoDB.Bson.Serialization.Attributes;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;
<#if NeedsSharedOwnedChildren() || NeedsSharedArrayChildren() || NeedsSharedEnumerationChildren()
#>using <# Project.Namespace#>.Contracts.Shared;
<#end#>using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

[BsonIgnoreExtraElements]
public class <# Definition.Name#>Model : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public DateTime? DeletedAt { get; set; }
<#for entry in Entries
#>	public <#cs Write(entry.IsArray ? "List<" : "")#><# entry.EntryType#><#cs Write(entry.IsArray ? ">" : entry.IsNullable ? "?" : "")#> <# entry.Field#><#cs Write(entry.IsReference ? "Id" : "")#> { get; set; }<#if entry.EntryType == "string"
#> = string.Empty;<#elseif entry.IsArray
#> = [];<#elseif entry.IsOwnedType && !entry.IsNullable
#> = new();<#end#>
<#end
#>}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>