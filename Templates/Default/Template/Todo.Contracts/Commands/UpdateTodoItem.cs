using System;
using System.Collections.Generic;

namespace <#cs Write(Project.Namespace)#>.Contracts.Commands;

public class Update<#cs Write(Definition.Name)#>(
	Guid id,
<#cs WriteContractParameter()#>)
{
	public Guid Id { get; } = id;
<#cs WriteContractProperties()#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>