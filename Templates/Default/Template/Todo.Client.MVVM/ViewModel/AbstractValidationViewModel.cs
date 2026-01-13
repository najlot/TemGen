using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Validation;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;

public abstract class AbstractValidationViewModel : AbstractViewModel, INotifyDataErrorInfo
{
	private Action _validateAction = () => { };
	private IEnumerable<ValidationResult> _errors = [];
	private IEnumerable<string> _previousProperties = [];

	protected void RaiseErrorsChanged(string propertyName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

	public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

	protected void SetValidation<T>(ValidationList<T> validations, bool validate = true) where T : AbstractValidationViewModel
	{
		if (!typeof(T).IsAssignableFrom(GetType()))
		{
			throw new InvalidOperationException($"{typeof(T).FullName} is not assignable from {GetType().FullName}");
		}

		_validateAction = async () =>
		{
			Errors = await validations.ValidateAsync((T)this);
			RaisePropertyChanged(nameof(HasErrors));
		};

		if (validate)
		{
			_validateAction();
		}
	}

	protected override bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
	{
		if (base.Set(ref oldValue, newValue, propertyName))
		{
			_validateAction();
			return true;
		}

		return false;
	}

	protected override bool Set<T>(string propertyName, ref T oldValue, T newValue)
	{
		if (base.Set(propertyName, ref oldValue, newValue))
		{
			_validateAction();
			return true;
		}

		return false;
	}

	private bool _hasErrors = false;
	public bool HasErrors { get => Errors.Any() || _hasErrors; set => _hasErrors = value; }

	public IEnumerable GetErrors(string propertyName)
	{
		foreach (var err in Errors)
		{
			if (string.IsNullOrEmpty(err.PropertyName) || err.PropertyName == propertyName)
			{
				yield return err;
			}
		}
	}

	public IEnumerable<ValidationResult> Errors
	{
		get => _errors;
		set
		{
			_errors = value;

			foreach (var propertyName in _previousProperties)
			{
				RaiseErrorsChanged(propertyName);
			}

			var propertyNames = _errors.Select(err => err.PropertyName).Distinct();

			foreach (var propertyName in propertyNames)
			{
				RaiseErrorsChanged(propertyName);
			}

			_previousProperties = propertyNames;
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>