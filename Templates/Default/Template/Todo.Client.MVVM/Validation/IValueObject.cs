using System.Collections.Generic;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM.Validation;

public interface IValueObject
{
	IEnumerable<ValidationResult> Validate();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>