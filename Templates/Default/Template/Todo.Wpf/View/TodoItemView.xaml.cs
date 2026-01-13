using System.Windows.Controls;

namespace <#cs Write(Project.Namespace)#>.Wpf.View;

public partial class <#cs Write(Definition.Name)#>View : UserControl
{
	public <#cs Write(Definition.Name)#>View()
	{
		InitializeComponent();
	}
}<#cs SetOutputPath(Definition.IsEnumeration)#>