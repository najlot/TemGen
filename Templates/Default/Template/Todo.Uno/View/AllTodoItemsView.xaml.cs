using Microsoft.UI.Xaml.Controls;

namespace <#cs Write(Project.Namespace)#>.Uno.View;

public sealed partial class All<#cs Write(Definition.Name)#>sView : UserControl
{
	public All<#cs Write(Definition.Name)#>sView()
	{
		this.InitializeComponent();
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>
