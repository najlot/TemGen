using System;
using System.Collections.Generic;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Validation;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Validation;

public class <#cs Write(Definition.Name)#>Validation : ValidationBase<<#cs Write(Definition.Name)#>ViewModel>
{
	public override IEnumerable<ValidationResult> Validate(<#cs Write(Definition.Name)#>ViewModel o)
	{
		return [];
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration)#>