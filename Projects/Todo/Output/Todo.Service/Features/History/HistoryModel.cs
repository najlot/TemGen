using MongoDB.Bson.Serialization.Attributes;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.History;

[BsonIgnoreExtraElements]
public class HistoryModel : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public Guid EntityId { get; set; }
	public Guid UserId { get; set; }
	public string Username { get; set; } = string.Empty;
	public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
	public string Changes { get; set; } = string.Empty;
}
