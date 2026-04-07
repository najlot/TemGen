using System;
using System.Collections.Generic;

namespace <# Project.Namespace#>.Contracts.Events;

public class <# Definition.Name#>Updated
{
	public Guid Id { get; set; }
<#cs WriteContractProperties()#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>