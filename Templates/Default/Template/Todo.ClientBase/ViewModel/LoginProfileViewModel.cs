using <#cs Write(Project.Namespace)#>.ClientBase.Models;
using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class LoginProfileViewModel() : AbstractViewModel
{
	private ProfileBase _profile;
	public ProfileBase Profile
	{
		get => _profile;
		set => Set(ref _profile, value);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>