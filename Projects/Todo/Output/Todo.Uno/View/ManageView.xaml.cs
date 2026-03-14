using Microsoft.UI.Xaml.Controls;
using Todo.ClientBase.ViewModel;

namespace Todo.Uno.View;

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

