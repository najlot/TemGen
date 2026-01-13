using NUnit.Framework;
using Todo.Service.Mappings;

namespace Todo.Service.Test.Mappings;

public class MapTest
{
	[Test]
	public void Map_must_be_valid()
	{
		new Najlot.Map.Map().RegisterDataMappings().Validate();
	}
}