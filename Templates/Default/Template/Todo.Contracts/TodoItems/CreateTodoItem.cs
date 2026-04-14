using System;
using System.Collections.Generic;
<#if NeedsSharedEnumerationChildren() || NeedsSharedOwnedChildren() || NeedsSharedArrayChildren()
#>using <# Project.Namespace#>.Contracts.Shared;
<#end#>
namespace <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

public class Create<# Definition.Name#>
{
	public Guid Id { get; set; }
<#cs WriteContractProperties()#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>