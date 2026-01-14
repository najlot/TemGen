using NUnit.Framework;
using <#cs Write(Project.Namespace)#>.Service.Mappings;

namespace <#cs Write(Project.Namespace)#>.Service.Test.Mappings;

public class MapTest
{
	[Test]
	public void Map_must_be_valid()
	{
		new Najlot.Map.Map().Register<#cs Write(Project.Namespace.Replace(".", ""))#>ServiceMappings().Validate();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>