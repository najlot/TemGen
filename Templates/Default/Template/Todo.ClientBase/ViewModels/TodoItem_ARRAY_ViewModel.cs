using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.ViewModels;

public <#if Entries.Any(e => e.IsArray)
#>partial <#end#>class <# Definition.Name#>ViewModel : ValidationViewModelBase
{
	public int Id { get; set => Set(ref field, value); }

<#for entry in Entries.Where(e => !e.IsArray)
#><#if entry.EntryType == "DateTime"
#>	private <# entry.EntryType#><#cs Write(entry.IsNullable ? "?" : "")#> _<# entry.FieldLow#>;

	public <# entry.EntryType#><#cs Write(entry.IsNullable ? "?" : "")#> <# entry.Field#>
	{
		get => _<# entry.FieldLow#>;
		set
		{
			if (Set(ref _<# entry.FieldLow#>, value))
			{
				RaisePropertiesChanged(nameof(<# entry.Field#>Date), nameof(<# entry.Field#>Time));
			}
		}
	}

	public DateTimeOffset? <# entry.Field#>Date
	{
		get => ToDateTimeOffset(<# entry.Field#>);
		set
		{
<#if entry.IsNullable
#>			if (value is null)
			{
				<# entry.Field#> = null;
				return;
			}

			<# entry.Field#> = CombineDateAndTime(value.Value.Date, <# entry.Field#>?.TimeOfDay);
<#else
#>			if (value is null)
			{
				return;
			}

			<# entry.Field#> = CombineDateAndTime(value.Value.Date, <# entry.Field#>.TimeOfDay);
<#end#>		}
	}

	public TimeSpan? <# entry.Field#>Time
	{
<#if entry.IsNullable
#>		get => <# entry.Field#>?.TimeOfDay;
		set
		{
			if (value is null)
			{
				if (<# entry.Field#> is null)
				{
					return;
				}

				<# entry.Field#> = CombineDateAndTime(<# entry.Field#>.Value.Date, null);
				return;
			}

			<# entry.Field#> = CombineDateAndTime(<# entry.Field#>?.Date ?? DateTime.Today, value);
		}
<#else
#>		get => <# entry.Field#>.TimeOfDay;
		set
		{
			if (value is null)
			{
				return;
			}

			<# entry.Field#> = CombineDateAndTime(<# entry.Field#>.Date, value);
		}
<#end#>	}
<#else
#>	public <# entry.EntryType#><#if entry.IsOwnedType
#>ViewModel<#end#><#cs Write(entry.IsNullable ? "?" : "")#> <# entry.Field#> { get; set => Set(ref field, value); }<#if !entry.IsNullable && entry.EntryType.ToLower() == "string"
#> = string.Empty;<#end#>
<#end#><#end#>

<#if Entries.Any(e => e.EntryType == "DateTime")
#>	private static DateTime CombineDateAndTime(DateTime datePart, TimeSpan? timePart)
		=> datePart.Date + (timePart ?? TimeSpan.Zero);

	private static DateTimeOffset? ToDateTimeOffset(DateTime? value)
		=> value is null ? null : new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified), TimeSpan.Zero);

<#end#>
	public Guid ParentId { get; set => Set( ref field, value); }
	public bool IsBusy { get; set => Set(ref field, value); }

	public <# Definition.Name#>ViewModel(ViewModelBaseParameters<<# Definition.Name#>ViewModel> parameters) : base(parameters)
	{
		SaveCommand = new AsyncCommand(RequestSave, t => HandleError(t.Exception));
		DeleteCommand = new AsyncCommand(RequestDelete, t => HandleError(t.Exception));
	}

	public void InitializeTracking(ChangeTracker changeTracker)
	{
		ChangeVisitor = changeTracker;
<#for entry in Entries.Where(e => e.IsOwnedType)
#>		Initialize<# entry.Field#>Tracking();
<#end#>
<#for entry in Entries.Where(e => e.IsArray)
#>		Initialize<# entry.Field#>Tracking();
<#end#>	}

<#for entry in Entries.Where(e => e.IsOwnedType)
#>	private void Initialize<# entry.Field#>Tracking()
	{
		<# entry.Field#>?.InitializeTracking((ChangeTracker)ChangeVisitor!);
	}

<#end#>

	private readonly List<Func<<# Definition.Name#>ViewModel, Task>> _onSaveRequested = [];
	public void OnSaveRequested(Func<<# Definition.Name#>ViewModel, Task> func) => _onSaveRequested.Add(func);

	public AsyncCommand SaveCommand { get; }
	private async Task RequestSave()
	{
		foreach (var func in _onSaveRequested)
		{
			await func(this);
		}
	}

	private readonly List<Func<<# Definition.Name#>ViewModel, Task<bool>>> _onDeleteRequested = [];
	public void OnDeleteRequested(Func<<# Definition.Name#>ViewModel, Task<bool>> func) => _onDeleteRequested.Add(func);

	public AsyncCommand DeleteCommand { get; }
	private async Task RequestDelete()
	{
		bool navigateBack = true;

		foreach (var func in _onDeleteRequested)
		{
			if (!await func(this))
			{
				navigateBack = false;
			}
		}

		if (navigateBack)
		{
			await NavigationService.NavigateBack();
		}
	}

	protected override IEnumerable<ValidationResult> Validate(string? propertyName)
	{
		return [];
	}

	protected override bool ShouldIgnorePropertyForChangesAndValidation(string? propertyName)
		=> base.ShouldIgnorePropertyForChangesAndValidation(propertyName)
			|| propertyName is nameof(Id) or nameof(ParentId) or nameof(IsBusy);
}<#cs
SetOutputPath(!Definition.IsArray);
RelativePath = RelativePath.Replace("_ARRAY_", "");
#>