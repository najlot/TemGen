using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.Filters;

[BsonIgnoreExtraElements]
public sealed class FilterModel : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public ItemType TargetType { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool IsDefault { get; set; }
	public List<FilterCondition> Conditions { get; set; } = [];
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>