using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Todo.Contracts.Filters;
using Todo.Contracts.Shared;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.Filters;

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
