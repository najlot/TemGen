using MongoDB.Bson.Serialization.Attributes;
using Todo.Contracts.TodoItems;
using Todo.Service.Infrastructure.Persistence;

namespace Todo.Service.Features.TodoItems;

[BsonIgnoreExtraElements]
public class TodoItemModel : IEntityModel
{
	[BsonId]
	public Guid Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? DeletedAt { get; set; }
	public Guid CreatedBy { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public Guid AssignedToId { get; set; }
	public TodoItemStatus Status { get; set; }
	public DateTime DueDate { get; set; }
	public string Priority { get; set; } = string.Empty;
	public List<ChecklistTask> Checklist { get; set; } = [];
}