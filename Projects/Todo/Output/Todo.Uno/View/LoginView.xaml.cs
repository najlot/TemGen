using Microsoft.UI.Xaml.Controls;
using Todo.ClientBase.ViewModel;

namespace Todo.Uno.View;

public sealed partial class LoginView : UserControl
{
	public LoginView()
	{
		this.InitializeComponent();
	}

	private void PasswordBox_PasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		if (DataContext is LoginViewModel vm)
		{
			vm.Password = PasswordBox.Password;
		}
	}
}

