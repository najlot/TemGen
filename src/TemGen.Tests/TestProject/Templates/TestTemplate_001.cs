namespace Demo;
<#cs Write("First")#>
public class TestTemplate
{
<#ref Type.Name#>
	public string Name => "value";
<#js write("Third");#>
}