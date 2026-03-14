using Avalonia.Controls;
using Avalonia.Interactivity;

namespace <#cs Write(Project.Namespace)#>.Avalonia.View;

public partial class LoginView : UserControl
{
	public LoginView()
	{
		InitializeComponent();

		Loaded += LoginView_Loaded;
	}

	private void LoginView_Loaded(object? sender, RoutedEventArgs e)
	{
		Loaded -= LoginView_Loaded;

		UsernameTextBox.Focus();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
