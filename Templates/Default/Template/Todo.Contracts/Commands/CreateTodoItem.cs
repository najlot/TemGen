using System;
using System.Collections.Generic;

namespace <# Project.Namespace#>.Contracts.Commands;

public class Create<# Definition.Name#>
{
	public Guid Id { get; set; }
<#cs WriteContractProperties()#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>