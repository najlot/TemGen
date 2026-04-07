using Najlot.Audit;
using Najlot.Audit.Attributes;

namespace Todo.Service.Features.Users;

[AuditProvider]
internal partial class UserAuditProvider
{
	[AuditIgnore(nameof(model.Id))]
	[AuditIgnore(nameof(model.PasswordHash))]
	[AuditIgnore(nameof(model.DeletedAt))]
	[PostAudit(nameof(PostGetPropertyValues))]
	public static partial IEnumerable<PropertyValue> GetPropertyValues(UserModel model);

	private static IEnumerable<PropertyValue> PostGetPropertyValues(UserModel model)
	{
		yield return new PropertyValue("IsDeleted", model.DeletedAt.HasValue);
	}
}
