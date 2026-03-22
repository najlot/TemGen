using Avalonia.Controls;

namespace <# Project.Namespace#>.Avalonia.Views;

public partial class <# Definition.Name#>View : UserControl
{
	public <# Definition.Name#>View()
	{
		InitializeComponent();
	}
}
<#cs
if (!Definition.IsOwnedType)
{
	RelativePath = "";
}
else
{
	RelativePath = RelativePath.Replace("TodoItem_OWNED_", Definition.Name).Replace("Todo", Project.Namespace);
}
#>