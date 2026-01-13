using System.Collections.Generic;

namespace Todo.Client.MVVM.Validation;

public interface IValueObject
{
	IEnumerable<ValidationResult> Validate();
}