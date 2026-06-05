using Avalonia.Controls;

namespace <# Project.Namespace#>.Avalonia.Views;

public partial class <# Definition.Name#>sView : UserControl
{
	public <# Definition.Name#>sView()
	{
		InitializeComponent();
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>