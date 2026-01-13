using NUnit.Framework;

namespace Todo.ClientBase.Test.Mappings;

public class MapTest
{
	[Test]
	public void Map_must_be_valid()
	{
		new Najlot.Map.Map().RegisterViewModelMappings().Validate();
	}
}