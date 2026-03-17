using System;
using System.Collections.Generic;

namespace <# Project.Namespace#>.Contracts.Events;

public class <# Definition.Name#>Created(
	Guid id,
<#cs WriteContractParameter()#>)
{
	public Guid Id { get; } = id;
<#cs WriteContractProperties()#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>