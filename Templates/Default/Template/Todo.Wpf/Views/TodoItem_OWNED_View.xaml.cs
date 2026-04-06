using System.Windows.Controls;

namespace <# Project.Namespace#>.Wpf.Views;

public partial class <# Definition.Name#>sView : UserControl
{
	public <# Definition.Name#>sView()
	{
		InitializeComponent();
	}
}<#cs
SetOutputPath(!Definition.IsOwnedType);
RelativePath = RelativePath.Replace("_OWNED_", "");
#>