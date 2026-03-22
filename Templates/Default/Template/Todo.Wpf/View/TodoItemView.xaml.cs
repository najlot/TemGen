using System.Windows.Controls;

namespace <# Project.Namespace#>.Wpf.View;

public partial class <# Definition.Name#>View : UserControl
{
	public <# Definition.Name#>View()
	{
		InitializeComponent();
	}
}<#cs SetOutputPath(Definition.IsEnumeration || Definition.IsOwnedType)#>