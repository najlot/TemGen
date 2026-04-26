using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Filters;
using Todo.Client.Localisation;
using Todo.ClientBase.Filters;
using Todo.Contracts.Filters;
using Todo.Contracts.Shared;
using Todo.Contracts.Notes;

namespace Todo.ClientBase.Notes;

public class NoteFilterViewModel : EntityFilterEditorViewModel
{
	public NoteFilterViewModel(
		IFilterService filterService,
		ViewModelBaseParameters<NoteFilterViewModel> parameters) : base(filterService, ItemType.Note, parameters)
	{
	}

	protected override Task<IReadOnlyList<FilterFieldOption>> BuildFilterFieldOptionsAsync()
	{
		return Task.FromResult<IReadOnlyList<FilterFieldOption>>(
		[
			new FilterFieldOption
			{
				Key = "Title",
				Label = NoteLoc.Title,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "Content",
				Label = NoteLoc.Content,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "Color",
				Label = NoteLoc.Color,
				Kind = FilterFieldKind.Option,
				Operators = FilterOperatorCatalog.CreateEqualityOptions(false),
				Values = [.. Enum.GetValues<PredefinedColor>().Select(value => new FilterValueOption
				{
					Value = value.ToString(),
					Label = value.ToString(),
				})],
			},
		]);
	}
}
