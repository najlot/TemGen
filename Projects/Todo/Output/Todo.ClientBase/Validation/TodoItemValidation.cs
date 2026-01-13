using System;
using System.Collections.Generic;
using Todo.Client.MVVM.Validation;
using Todo.ClientBase.ViewModel;

namespace Todo.ClientBase.Validation;

public class TodoItemValidation : ValidationBase<TodoItemViewModel>
{
	public override IEnumerable<ValidationResult> Validate(TodoItemViewModel o)
	{
		return [];
	}
}