namespace <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;

public class YesNoPageViewModel : AbstractPopupViewModel<bool>
{
	public string Title { get; set; }
	public string Message { get; set; }

	public RelayCommand YesCommand { get; }
	public RelayCommand NoCommand { get; }

	public YesNoPageViewModel()
	{
		YesCommand = new RelayCommand(() => SetResult(true));
		NoCommand = new RelayCommand(() => SetResult(false));
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>