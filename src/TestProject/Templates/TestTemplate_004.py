using <#py write(project.Namespace)#>.Model;

namespace <#py write(project.Namespace)#>
{
	public class <#py write(definition.Name)#>
	{
<#py
for entry in entries:
	write_line("		" + entry.EntryType + " " + entry.Field + " { get; set; }");

set_result(get_result().rstrip());
#>
	}
}
<#py relative_path = relative_path.replace("TestTemplate", definition.Name)#>