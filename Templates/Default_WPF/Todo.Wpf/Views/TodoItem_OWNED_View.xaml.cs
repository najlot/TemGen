using System.Windows.Controls;

namespace <# Project.Namespace#>.Wpf.Views;

public partial class <# Definition.Name#>View : UserControl
{
	public <# Definition.Name#>View()
	{
		InitializeComponent();
	}
}<#cs
SetOutputPath(!Definition.IsOwnedType);
RelativePath = RelativePath.Replace("_OWNED_", "");
#>