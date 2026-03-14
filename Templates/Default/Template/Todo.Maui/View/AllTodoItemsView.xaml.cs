namespace <#cs Write(Project.Namespace)#>.Maui.View;

public partial class All<#cs Write(Definition.Name)#>sView : ContentView
{
	public All<#cs Write(Definition.Name)#>sView()
	{
		InitializeComponent();
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>
