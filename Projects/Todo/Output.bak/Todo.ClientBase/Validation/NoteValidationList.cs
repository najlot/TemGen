using System.Collections.Generic;
using Todo.Client.MVVM.Validation;
using Todo.ClientBase.ViewModel;

namespace Todo.ClientBase.Validation;

public class NoteValidationList : ValidationList<NoteViewModel>
{
	public NoteValidationList()
	{
		Add(new NoteValidation());
	}
}