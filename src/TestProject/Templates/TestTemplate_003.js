using <#js write(project.namespace)#>.Model;

namespace <#js write(project.namespace)#>
{
	public class <#js write(definition.name)#>
	{
<#js
for (i in entries)
{
	writeLine("		" + entries[i].entryType + " " + entries[i].field + " { get; set; }");
}

result = result.substring(0, result.length - 2);
#>
	}
}
<#js relativePath = relativePath.replace("TestTemplate", definition.name)#>