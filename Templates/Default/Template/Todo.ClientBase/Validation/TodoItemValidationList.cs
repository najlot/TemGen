using System.Collections.Generic;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Validation;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Validation;

public class <#cs Write(Definition.Name)#>ValidationList : ValidationList<<#cs Write(Definition.Name)#>ViewModel>
{
	public <#cs Write(Definition.Name)#>ValidationList()
	{
		Add(new <#cs Write(Definition.Name)#>Validation());
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration)#>