using <#cs Write(Project.Namespace)#>.Model;

namespace <#cs Write(Project.Namespace)#>
{
	public class <#cs Write(Definition.Name)#>
	{
<#cs
foreach(var entry in Entries)
{
	WriteLine("		" + entry.EntryType + " " + entry.Field + " { get; set; }");
}

Result = Result.TrimEnd();
#>
	}
}
<#cs RelativePath = RelativePath.Replace("TestTemplate", Definition.Name)#>