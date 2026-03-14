using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Todo.Client.MVVM;

public abstract class AbstractValidationViewModel : AbstractViewModel, INotifyDataErrorInfo
{
	public record ValidationResult(string PropertyName, string Text);

	public bool HasErrors { get; private set => base.Set(ref field, value, RaiseHasErrorsChanged); }

	public event Action? HasErrorsChanged;
	public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

	private readonly List<ValidationResult> _errors = [];
	public IEnumerable GetErrors(string? propertyName) => _errors
		.Where(e => propertyName == null || e.PropertyName == propertyName)
		.Select(e => e.Text);

	private void RaiseHasErrorsChanged()
	{
		HasErrorsChanged?.Invoke();
	}

	protected void RaiseErrorsChanged(string? propertyName)
	{
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}

	protected override bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string? propertyName = null)
	{
		if (base.Set(ref oldValue, newValue, propertyName))
		{
			if (!ShouldIgnorePropertyForChangesAndValidation(propertyName))
			{
				RunValidation(propertyName);
			}

			return true;
		}

		return false;
	}

	protected void ValidateAll() => RunValidation(null);

	protected static bool ShouldValidate(string? propertyName, string propertyToValidate)
		=> string.IsNullOrWhiteSpace(propertyName) || propertyName == propertyToValidate;

	protected static bool ShouldValidate(string? propertyName, string[] propertiesToValidate)
		=> string.IsNullOrWhiteSpace(propertyName) || propertiesToValidate.Any(p => p == propertyName);

	protected virtual bool ShouldIgnorePropertyForChangesAndValidation(string? propertyName) => propertyName is nameof(HasErrors);

	protected override bool ShouldTrackChange(string? propertyName)
		=> !ShouldIgnorePropertyForChangesAndValidation(propertyName);

	private void RunValidation(string? propertyName)
	{
		var errors = Validate(propertyName).ToArray();

		var propertyNames = _errors
			.Select(e => e.PropertyName)
			.Concat(errors.Select(e => e.PropertyName))
			.Distinct()
			.ToArray();

		if (string.IsNullOrEmpty(propertyName))
		{
			_errors.Clear();
		}
		else if (errors.Length == 0)
		{
			_errors.RemoveAll(e => e.PropertyName == propertyName);
		}
		else
		{
			_errors.RemoveAll(e => errors.Any(err => err.PropertyName == e.PropertyName));
		}

		_errors.AddRange(errors.Where(e => !string.IsNullOrEmpty(e.Text)));

		HasErrors = _errors.Count > 0;

		foreach (var name in propertyNames)
		{
			RaiseErrorsChanged(name);
		}
	}

	protected abstract IEnumerable<ValidationResult> Validate(string? propertyName);

	protected static ValidationResult Result(string propertyName, Func<bool> isValid, Func<string> getErrorMessage)
		=> isValid() ? Okay(propertyName) : Error(propertyName, getErrorMessage());

	protected static ValidationResult Result(string propertyName, Func<bool> isValid, string errorMessage)
		=> isValid() ? Okay(propertyName) : Error(propertyName, errorMessage);

	protected static ValidationResult Result(string propertyName, bool isValid, string errorMessage)
		=> isValid ? Okay(propertyName) : Error(propertyName, errorMessage);

	protected static ValidationResult Okay(string propertyName) => new(propertyName, string.Empty);
	protected static ValidationResult Error(string propertyName, string text) => new(propertyName, text);
}