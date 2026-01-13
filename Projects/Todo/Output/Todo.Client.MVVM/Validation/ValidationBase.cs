using System.Collections.Generic;

namespace Todo.Client.MVVM.Validation;

public abstract class ValidationBase<T>
{
	protected ValidationResult Info(string propertyName, string info)
	{
		return new ValidationResult(ValidationSeverity.Info, propertyName, info);
	}

	protected ValidationResult Warning(string propertyName, string warning)
	{
		return new ValidationResult(ValidationSeverity.Warning, propertyName, warning);
	}

	protected ValidationResult Error(string propertyName, string error)
	{
		return new ValidationResult(ValidationSeverity.Error, propertyName, error);
	}

	public abstract IEnumerable<ValidationResult> Validate(T o);
}