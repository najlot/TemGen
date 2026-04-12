using Najlot.Audit;
using Najlot.Audit.Attributes;
using Todo.Contracts.TodoItems;

namespace Todo.Service.Features.TodoItems;

[AuditProvider]
internal partial class TodoItemAuditProvider
{
	[AuditIgnore("Checklist.Id")]
	[AuditIgnore(nameof(model.Id))]
	[AuditIgnore(nameof(model.DeletedAt))]
	[PostAudit(nameof(PostGetPropertyValues))]
	public static partial IEnumerable<PropertyValue> GetPropertyValues(TodoItemModel model);

	private static IEnumerable<PropertyValue> PostGetPropertyValues(TodoItemModel model)
	{
		yield return new PropertyValue("IsDeleted", model.DeletedAt.HasValue);
	}
}
