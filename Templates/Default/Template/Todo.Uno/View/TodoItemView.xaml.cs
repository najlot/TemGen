using Microsoft.UI.Xaml.Controls;

namespace <#cs Write(Project.Namespace)#>.Uno.View;

public sealed partial class <#cs Write(Definition.Name)#>View : UserControl
{
	public <#cs Write(Definition.Name)#>View()
	{
		this.InitializeComponent();
	}
}
<#cs SetOutputPath(Definition.IsEnumeration)#>
