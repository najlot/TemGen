using System;
using System.Collections.Generic;

namespace <# Project.Namespace#>.Contracts.Commands;

public class Create<# Definition.Name#>(
	Guid id,
<#cs WriteContractParameter()#>)
{
	public Guid Id { get; } = id;
<#cs WriteContractProperties()#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>