using System;

namespace <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

public class <# Definition.Name#>Deleted(Guid id)
{
	public Guid Id { get; } = id;
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>