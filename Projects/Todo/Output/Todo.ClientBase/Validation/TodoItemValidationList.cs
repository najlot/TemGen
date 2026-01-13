using System.Collections.Generic;
using Todo.Client.MVVM.Validation;
using Todo.ClientBase.ViewModel;

namespace Todo.ClientBase.Validation;

public class TodoItemValidationList : ValidationList<TodoItemViewModel>
{
	public TodoItemValidationList()
	{
		Add(new TodoItemValidation());
	}
}