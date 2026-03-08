using System.Windows.Controls;

namespace Todo.Wpf.View;

public partial class LoginView : UserControl
{
	public LoginView()
	{
		InitializeComponent();

		Loaded += LoginView_Loaded;
	}

	private void LoginView_Loaded(object sender, System.Windows.RoutedEventArgs e)
	{
		Loaded -= LoginView_Loaded;

		UsernameTextBox.Focus();
	}
}