using MongoDB.Bson.Serialization.Attributes;
using Todo.Contracts;

namespace Todo.Service.Model;

[BsonIgnoreExtraElements]
public class NoteModel : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public DateTime? DeletedAt { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public PredefinedColor Color { get; set; }
}