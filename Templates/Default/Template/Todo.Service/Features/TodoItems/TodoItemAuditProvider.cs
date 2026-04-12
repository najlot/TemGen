using Najlot.Audit;
using Najlot.Audit.Attributes;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

[AuditProvider]
internal partial class <# Definition.Name#>AuditProvider
{
<#cs foreach (var entry in Entries)
{
	if (!entry.IsArray)
	{
		continue;
	}

	WriteLine($"\t[AuditIgnore(\"{entry.Field}.Id\")]");
}
#>	[AuditIgnore(nameof(model.Id))]
	[AuditIgnore(nameof(model.DeletedAt))]
	[PostAudit(nameof(PostGetPropertyValues))]
	public static partial IEnumerable<PropertyValue> GetPropertyValues(<# Definition.Name#>Model model);

	private static IEnumerable<PropertyValue> PostGetPropertyValues(<# Definition.Name#>Model model)
	{
		yield return new PropertyValue("IsDeleted", model.DeletedAt.HasValue);
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>