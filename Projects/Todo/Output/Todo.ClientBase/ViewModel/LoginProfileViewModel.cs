using Todo.ClientBase.Models;
using Todo.Client.MVVM.ViewModel;

namespace Todo.ClientBase.ViewModel;

public class LoginProfileViewModel() : AbstractViewModel
{
	private ProfileBase _profile;
	public ProfileBase Profile
	{
		get => _profile;
		set => Set(ref _profile, value);
	}
}