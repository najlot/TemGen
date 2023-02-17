using <#py write(project.Namespace)#>.Model;

namespace <#py write(project.Namespace)#>
{
	public class <#py write(definition.Name)#>
	{
<#py
for entry in entries:
	writeLine("		" + entry.EntryType + " " + entry.Field + " { get; set; }");

result = result.rstrip();
#>
	}
}
<#py relativePath = relativePath.replace("TestTemplate", definition.Name)#>