using System;
using System.Collections.Generic;
using System.Linq;
using <# Project.Namespace#>.Client.Data.Filters;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Contracts.Filters;

namespace <# Project.Namespace#>.ClientBase.Filters;

public sealed class FilterConditionViewModel : AbstractViewModel
{
	private readonly IReadOnlyList<FilterFieldOption> _availableFields;
	private string _selectedFieldKey = string.Empty;
	private FilterOperator _selectedOperator = FilterOperator.Equals;
	private string _value = string.Empty;

	public FilterConditionViewModel(IReadOnlyList<FilterFieldOption> availableFields, FilterCondition? condition = null)
	{
		_availableFields = availableFields ?? [];

		_selectedFieldKey = condition?.Field ?? string.Empty;
		_selectedOperator = condition?.Operator ?? FilterOperator.Equals;
		_value = condition?.Value ?? string.Empty;

		EnsureValidSelection();
	}

	public string SelectedFieldKey
	{
		get => _selectedFieldKey;
		set => Set(ref _selectedFieldKey, value, OnFieldChanged);
	}

	public FilterOperator SelectedOperator
	{
		get => _selectedOperator;
		set => Set(ref _selectedOperator, value, OnOperatorChanged);
	}

	public string Value
	{
		get => _value;
		set => Set(ref _value, value ?? string.Empty, () => RaisePropertyChanged(nameof(SelectedValueOption)));
	}

	public FilterOperatorOption? SelectedOperatorOption
	{
		get => AvailableOperators.FirstOrDefault(option => option.Value == SelectedOperator);
		set
		{
			if (value is not null)
			{
				SelectedOperator = value.Value;
			}
		}
	}

	public FilterValueOption? SelectedValueOption
	{
		get => AvailableValues.FirstOrDefault(option => option.Value == Value);
		set
		{
			if (value is not null)
			{
				Value = value.Value;
			}
			else if (AvailableValues.Count == 0)
			{
				Value = string.Empty;
			}
		}
	}

	public IReadOnlyList<FilterFieldOption> AvailableFields => _availableFields;

	public FilterFieldOption? SelectedField => _availableFields.FirstOrDefault(f => f.Key == SelectedFieldKey);

	public IReadOnlyList<FilterOperatorOption> AvailableOperators => SelectedField?.Operators ?? [];

	public IReadOnlyList<FilterValueOption> AvailableValues => SelectedField?.Values ?? [];

	public bool UsesSelectableValues => AvailableValues.Count > 0;

	public bool RequiresValue => SelectedOperator.RequiresValue();

	public bool ShowTextValueEditor => RequiresValue && !UsesSelectableValues;

	public bool ShowOptionValueEditor => RequiresValue && UsesSelectableValues;

	private void OnFieldChanged()
	{
		EnsureValidSelection();

		RaisePropertiesChanged(
			nameof(SelectedField),
			nameof(AvailableOperators),
			nameof(AvailableValues),
			nameof(RequiresValue),
			nameof(UsesSelectableValues),
			nameof(ShowTextValueEditor),
			nameof(ShowOptionValueEditor),
			nameof(SelectedOperatorOption),
			nameof(SelectedValueOption)
		);
	}

	private void OnOperatorChanged()
	{
		EnsureValidSelection();

		RaisePropertiesChanged(
			nameof(RequiresValue),
			nameof(ShowTextValueEditor),
			nameof(ShowOptionValueEditor),
			nameof(SelectedOperatorOption)
		);
	}

	private void EnsureValidSelection()
	{
		if (_availableFields.All(f => f.Key != _selectedFieldKey))
		{
			_selectedFieldKey = _availableFields.FirstOrDefault()?.Key ?? string.Empty;
			RaisePropertyChanged(nameof(SelectedFieldKey));
		}

		if (AvailableOperators.All(o => o.Value != _selectedOperator))
		{
			_selectedOperator = AvailableOperators.FirstOrDefault()?.Value ?? FilterOperator.Equals;
			RaisePropertiesChanged(nameof(SelectedOperator), nameof(SelectedOperatorOption));
		}

		if (!RequiresValue)
		{
			if (!string.IsNullOrEmpty(_value))
			{
				_value = string.Empty;
				RaisePropertiesChanged(nameof(Value), nameof(SelectedValueOption));
			}
			return;
		}

		if (UsesSelectableValues && AvailableValues.All(v => v.Value != _value))
		{
			_value = AvailableValues.FirstOrDefault()?.Value ?? string.Empty;
			RaisePropertiesChanged(nameof(Value), nameof(SelectedValueOption));
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>