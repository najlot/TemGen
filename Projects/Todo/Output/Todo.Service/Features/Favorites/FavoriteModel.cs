using MongoDB.Bson.Serialization.Attributes;
using System;
using Todo.Contracts.Shared;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.Favorites;

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
