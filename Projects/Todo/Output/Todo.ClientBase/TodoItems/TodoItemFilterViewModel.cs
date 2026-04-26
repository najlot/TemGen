using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Filters;
using Todo.Client.Localisation;
using Todo.ClientBase.Filters;
using Todo.Contracts.Filters;
using Todo.Contracts.Shared;
using Todo.Contracts.TodoItems;
using Todo.Client.Data.Users;

namespace Todo.ClientBase.TodoItems;

public class TodoItemFilterViewModel : EntityFilterEditorViewModel
{
	private readonly IUserService _userService;

	public TodoItemFilterViewModel(
		IUserService userService,
		IFilterService filterService,
		ViewModelBaseParameters<TodoItemFilterViewModel> parameters) : base(filterService, ItemType.TodoItem, parameters)
	{
		_userService = userService;
	}

	protected override async Task<IReadOnlyList<FilterFieldOption>> BuildFilterFieldOptionsAsync()
	{
		var users = await _userService.GetItemsAsync();
		return 
		[
			new FilterFieldOption
			{
				Key = "Title",
				Label = TodoItemLoc.Title,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "Content",
				Label = TodoItemLoc.Content,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "CreatedAt",
				Label = TodoItemLoc.CreatedAt,
				Kind = FilterFieldKind.DateTime,
				Operators = FilterOperatorCatalog.CreateComparableOptions(false),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "CreatedBy",
				Label = TodoItemLoc.CreatedBy,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "AssignedToId",
				Label = TodoItemLoc.AssignedTo,
				Kind = FilterFieldKind.Option,
				Operators = FilterOperatorCatalog.CreateEqualityOptions(false),
				Values = [.. users.Select(item => new FilterValueOption
				{
					Value = item.Id.ToString(),
					Label = Convert.ToString(item.Username) ?? CommonLoc.Untitled,
				})],
			},
			new FilterFieldOption
			{
				Key = "Status",
				Label = TodoItemLoc.Status,
				Kind = FilterFieldKind.Option,
				Operators = FilterOperatorCatalog.CreateEqualityOptions(false),
				Values = [.. Enum.GetValues<TodoItemStatus>().Select(value => new FilterValueOption
				{
					Value = value.ToString(),
					Label = value.ToString(),
				})],
			},
			new FilterFieldOption
			{
				Key = "ChangedAt",
				Label = TodoItemLoc.ChangedAt,
				Kind = FilterFieldKind.DateTime,
				Operators = FilterOperatorCatalog.CreateComparableOptions(false),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "ChangedBy",
				Label = TodoItemLoc.ChangedBy,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
			new FilterFieldOption
			{
				Key = "Priority",
				Label = TodoItemLoc.Priority,
				Kind = FilterFieldKind.Text,
				Operators = FilterOperatorCatalog.CreateTextOptions(),
				Values = [],
			},
		];
	}
}
