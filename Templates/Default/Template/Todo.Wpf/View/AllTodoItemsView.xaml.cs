using System.Windows.Controls;

namespace <# Project.Namespace#>.Wpf.View;

public partial class All<# Definition.Name#>sView : UserControl
{
	public All<# Definition.Name#>sView()
	{
		InitializeComponent();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>