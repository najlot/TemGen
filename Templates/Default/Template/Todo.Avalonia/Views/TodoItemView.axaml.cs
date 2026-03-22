using Avalonia.Controls;

namespace <# Project.Namespace#>.Avalonia.Views;

public partial class <# Definition.Name#>View : UserControl
{
	public <# Definition.Name#>View()
	{
		InitializeComponent();
	}
}
<#cs SetOutputPath(Definition.IsEnumeration || Definition.IsOwnedType)#>