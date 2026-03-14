using Avalonia.Controls;

namespace <#cs Write(Project.Namespace)#>.Avalonia.View;

public partial class All<#cs Write(Definition.Name)#>sView : UserControl
{
	public All<#cs Write(Definition.Name)#>sView()
	{
		InitializeComponent();
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>
