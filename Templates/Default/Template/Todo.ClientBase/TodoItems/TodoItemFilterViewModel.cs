using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Filters;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.ClientBase.Filters;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>using <# Project.Namespace#>.Client.Data.<# definition.Name#>s;
<#end#>
namespace <# Project.Namespace#>.ClientBase.<# Definition.Name#>s;

public class <# Definition.Name#>FilterViewModel : EntityFilterEditorViewModel
{<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>
	private readonly I<# definition.Name#>Service _<# definition.NameLow#>Service;
<#end#>
	public <# Definition.Name#>FilterViewModel(
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>		I<# definition.Name#>Service <# definition.NameLow#>Service,
<#end#>		IFilterService filterService,
		ViewModelBaseParameters<<# Definition.Name#>FilterViewModel> parameters) : base(filterService, ItemType.<# Definition.Name#>, parameters)
	{
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>		_<# definition.NameLow#>Service = <# definition.NameLow#>Service;
<#end#>	}

	protected override <#if Entries.Where(e => e.IsReference).Any()
#>async <#end#>Task<IReadOnlyList<FilterFieldOption>> BuildFilterFieldOptionsAsync()
	{
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>		var <# definition.NameLow#>s = await _<# definition.NameLow#>Service.GetItemsAsync();
<#end#>		return <#if Entries.Where(e => e.IsReference).Any()
#>
		[
<#for entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsOwnedType))
#>			new FilterFieldOption
			{
				Key = "<#cs Write(GetFieldPropertyName(entry.Field, entry.IsReference))#>",
				Label = <# Definition.Name#>Loc.<# entry.Field#>,
				Kind = FilterFieldKind.<#cs Write(GetFilterFieldKind(entry.EntryType, entry.IsReference, entry.IsEnumeration))#>,
				Operators = <#if entry.EntryType == "string"#>FilterOperatorCatalog.CreateTextOptions()<#elseif IsComparableType(entry.EntryType)#>FilterOperatorCatalog.CreateComparableOptions(<#cs Write(AllowsEmptyFilter(entry.IsNullable, entry.EntryType) ? "true" : "false")#>)<#else#>FilterOperatorCatalog.CreateEqualityOptions(<#cs Write(AllowsEmptyFilter(entry.IsNullable, entry.EntryType) ? "true" : "false")#>)<#end#>,
				Values = <#if entry.IsReference
#>[.. <# entry.ReferenceTypeLow#>s.Select(item => new FilterValueOption
				{
					Value = item.Id.ToString(),
					Label = Convert.ToString(item.<#cs Write(Definitions.First(d => d.Name == entry.ReferenceType).Entries.First(e => !(e.IsOwnedType || e.IsKey || e.IsArray || e.IsReference || e.IsEnumeration)).Field)#>) ?? CommonLoc.Untitled,
				})]<#elseif entry.IsEnumeration
#>[.. Enum.GetValues<<# entry.EntryType#>>().Select(value => new FilterValueOption
				{
					Value = value.ToString(),
					Label = value.ToString(),
				})]<#elseif entry.EntryType == "bool"
#>[new FilterValueOption { Value = bool.TrueString, Label = bool.TrueString }, new FilterValueOption { Value = bool.FalseString, Label = bool.FalseString }]<#else#>[]<#end#>,
			},
<#end#>		];<#else#>Task.FromResult<IReadOnlyList<FilterFieldOption>>(
		[
<#for entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsOwnedType))
#>			new FilterFieldOption
			{
				Key = "<#cs Write(GetFieldPropertyName(entry.Field, entry.IsReference))#>",
				Label = <# Definition.Name#>Loc.<# entry.Field#>,
				Kind = FilterFieldKind.<#cs Write(GetFilterFieldKind(entry.EntryType, entry.IsReference, entry.IsEnumeration))#>,
				Operators = <#if entry.EntryType == "string"#>FilterOperatorCatalog.CreateTextOptions()<#elseif IsComparableType(entry.EntryType)#>FilterOperatorCatalog.CreateComparableOptions(<#cs Write(AllowsEmptyFilter(entry.IsNullable, entry.EntryType) ? "true" : "false")#>)<#else#>FilterOperatorCatalog.CreateEqualityOptions(<#cs Write(AllowsEmptyFilter(entry.IsNullable, entry.EntryType) ? "true" : "false")#>)<#end#>,
				Values = <#if entry.IsReference
#>[.. <# entry.ReferenceTypeLow#>s.Select(item => new FilterValueOption
				{
					Value = item.Id.ToString(),
					Label = Convert.ToString(item.<#cs Write(Definitions.First(d => d.Name == entry.ReferenceType).Entries.First(e => !(e.IsOwnedType || e.IsKey || e.IsArray || e.IsReference || e.IsEnumeration)).Field)#>) ?? CommonLoc.Untitled,
				})]<#elseif entry.IsEnumeration
#>[.. Enum.GetValues<<# entry.EntryType#>>().Select(value => new FilterValueOption
				{
					Value = value.ToString(),
					Label = value.ToString(),
				})]<#elseif entry.EntryType == "bool"
#>[new FilterValueOption { Value = bool.TrueString, Label = bool.TrueString }, new FilterValueOption { Value = bool.FalseString, Label = bool.FalseString }]<#else#>[]<#end#>,
			},
<#end#>		]);<#end#>
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>