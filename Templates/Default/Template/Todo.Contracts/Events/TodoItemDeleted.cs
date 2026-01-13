using System;

namespace <#cs Write(Project.Namespace)#>.Contracts.Events;

public class <#cs Write(Definition.Name)#>Deleted(Guid id)
{
	public Guid Id { get; } = id;
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>