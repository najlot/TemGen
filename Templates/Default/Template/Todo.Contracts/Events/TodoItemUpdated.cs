using System;
using System.Collections.Generic;

namespace <#cs Write(Project.Namespace)#>.Contracts.Events;

public class <#cs Write(Definition.Name)#>Updated(
	Guid id,
<#cs WriteContractParameter()#>)
{
	public Guid Id { get; } = id;
<#cs WriteContractProperties()#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>