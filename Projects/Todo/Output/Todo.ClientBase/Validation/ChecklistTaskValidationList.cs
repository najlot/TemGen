using System.Collections.Generic;
using Todo.Client.MVVM.Validation;
using Todo.ClientBase.ViewModel;

namespace Todo.ClientBase.Validation;

public class ChecklistTaskValidationList : ValidationList<ChecklistTaskViewModel>
{
	public ChecklistTaskValidationList()
	{
		Add(new ChecklistTaskValidation());
	}
}