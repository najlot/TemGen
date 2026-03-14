using Microsoft.UI.Xaml.Controls;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

namespace <#cs Write(Project.Namespace)#>.Uno.View;

public sealed partial class ManageView : UserControl
{
	public ManageView()
	{
		this.InitializeComponent();
	}

	private void NewPasswordBox_PasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		if (DataContext is ManageViewModel vm)
		{
			vm.Password = NewPasswordBox.Password;
		}
	}

	private void ConfirmPasswordBox_PasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		if (DataContext is ManageViewModel vm)
		{
			vm.PasswordConfirm = ConfirmPasswordBox.Password;
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
