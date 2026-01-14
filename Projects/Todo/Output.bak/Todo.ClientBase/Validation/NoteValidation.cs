using System;
using System.Collections.Generic;
using Todo.Client.MVVM.Validation;
using Todo.ClientBase.ViewModel;

namespace Todo.ClientBase.Validation;

public class NoteValidation : ValidationBase<NoteViewModel>
{
	public override IEnumerable<ValidationResult> Validate(NoteViewModel o)
	{
		return [];
	}
}