using NUnit.Framework;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Test.Mappings;

public class MapTest
{
	[Test]
	public void Map_must_be_valid()
	{
		new Najlot.Map.Map().Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientBaseMappings().Validate();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>