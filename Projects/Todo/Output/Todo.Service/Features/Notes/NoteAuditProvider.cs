using Najlot.Audit;
using Najlot.Audit.Attributes;
using Todo.Contracts.Notes;

namespace Todo.Service.Features.Notes;

[AuditProvider]
internal partial class NoteAuditProvider
{
	[AuditIgnore(nameof(model.Id))]
	[AuditIgnore(nameof(model.DeletedAt))]
	[PostAudit(nameof(PostGetPropertyValues))]
	public static partial IEnumerable<PropertyValue> GetPropertyValues(NoteModel model);

	private static IEnumerable<PropertyValue> PostGetPropertyValues(NoteModel model)
	{
		yield return new PropertyValue("IsDeleted", model.DeletedAt.HasValue);
	}
}
