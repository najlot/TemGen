using MongoDB.Bson.Serialization.Attributes;
using System;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Service.Infrastructure.Persistence;

namespace <# Project.Namespace#>.Service.Features.Favorites;

[BsonIgnoreExtraElements]
public sealed class FavoriteModel : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public ItemType TargetType { get; set; }
	public Guid ItemId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>