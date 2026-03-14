using Microsoft.UI.Xaml.Controls;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

namespace <#cs Write(Project.Namespace)#>.Uno.View;

public sealed partial class RegisterView : UserControl
{
	public RegisterView()
	{
		this.InitializeComponent();
	}

	private void PasswordBox_PasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		if (DataContext is RegisterViewModel vm)
		{
			vm.Password = PasswordBox.Password;
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
