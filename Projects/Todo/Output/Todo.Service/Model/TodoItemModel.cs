using MongoDB.Bson.Serialization.Attributes;
using Todo.Contracts;

namespace Todo.Service.Model;

[BsonIgnoreExtraElements]
public class TodoItemModel : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public DateTime? DeletedAt { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public string CreatedBy { get; set; } = string.Empty;
	public Guid AssignedToId { get; set; }
	public TodoItemStatus Status { get; set; }
	public DateTime ChangedAt { get; set; }
	public string ChangedBy { get; set; } = string.Empty;
	public string Priority { get; set; } = string.Empty;
	public List<ChecklistTask> Checklist { get; set; } = [];
}