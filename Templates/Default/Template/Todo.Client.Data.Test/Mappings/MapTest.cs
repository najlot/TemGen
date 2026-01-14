using NUnit.Framework;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Test.Mappings;

public class MapTest
{
	[Test]
	public void Map_must_be_valid()
	{
		new Najlot.Map.Map().Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientDataMappings().Validate();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>