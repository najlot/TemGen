namespace <#cs Write(Project.Namespace)#>.Maui.View;

public partial class <#cs Write(Definition.Name)#>View : ContentView
{
	public <#cs Write(Definition.Name)#>View()
	{
		InitializeComponent();
	}
}<#cs SetOutputPath(Definition.IsEnumeration)#>
