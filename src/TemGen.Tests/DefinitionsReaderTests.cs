using System.IO;
using System.Linq;
using Xunit;

namespace TemGen.Tests;

public class DefinitionsReaderTests
{
	[Fact]
	public void TestProjectMustBeReadCorrect()
	{
		var path = Directory.GetCurrentDirectory();

		while (!Directory.GetDirectories(path).Any(p => p.Contains("TestProject")))
		{
			path = Path.GetDirectoryName(path);
		}

		path = Path.Combine(path, "TestProject", "Definitions");

		var definitions = DefinitionsReader.ReadDefinitions(path);
		var definitionNames = definitions.Select(d => d.Name).ToList();

		Assert.Contains("OrderInfo", definitionNames);
		Assert.Contains("OrderStatus", definitionNames);
		Assert.Contains("Position", definitionNames);
		Assert.Contains("Customer", definitionNames);
		Assert.Contains("Order", definitionNames);
		Assert.DoesNotContain("Empty", definitionNames);

		foreach (var def in definitions)
		{
			var entriesNames = definitions.Select(d => d.Name).ToList();

			Assert.NotEqual("", def.Name);

			if (def.Name == "OrderInfo")
			{
				Assert.True(def.IsOwnedType);
			}

			if (def.Name == "OrderStatus")
			{
				Assert.True(def.IsEnumeration);
				Assert.True(!def.IsOwnedType);

				if (def.Name == "Position")
				{
					Assert.True(def.IsArray);
					Assert.True(!def.IsOwnedType);
				}

				if (def.Name == "Customer")
				{
					Assert.True(!def.IsOwnedType);
				}

				if (def.Name == "Order")
				{
					foreach (var e in def.Entries)
					{
						Assert.NotEmpty(e.Field);
						Assert.NotEmpty(e.EntryType);

						if (e.Field == "Id")
						{
							Assert.True(e.IsKey);
							Assert.True(!e.IsOwnedType);
							Assert.True(!e.IsArray);
							Assert.True(!e.IsEnumeration);
							Assert.True(e.ReferenceType == "");
						}

						if (e.Field == "Client")
						{
							Assert.True(e.EntryType == "int");
							Assert.True(!e.IsKey);
							Assert.True(!e.IsOwnedType);
							Assert.True(!e.IsArray);
							Assert.True(!e.IsEnumeration);
						}

						if (e.Field == "CreatedAt")
						{
							Assert.True(e.EntryType == "DateTime");
							Assert.True(!e.IsKey);
							Assert.True(!e.IsOwnedType);
							Assert.True(!e.IsArray);
							Assert.True(!e.IsEnumeration);
							Assert.True(!e.IsNullable);
						}

						if (e.Field == "ModifiedAt")
						{
							Assert.True(e.EntryType == "DateTime");
							Assert.True(!e.IsKey);
							Assert.True(!e.IsOwnedType);
							Assert.True(!e.IsArray);
							Assert.True(!e.IsEnumeration);
							Assert.True(e.IsNullable);
						}

						if (e.Field == "MoreInfo")
						{
							Assert.True(e.EntryType == "OrderInfo");
							Assert.True(e.IsOwnedType);
						}

						if (e.Field == "Status")
						{
							Assert.True(e.IsEnumeration);
							Assert.True(!e.IsOwnedType);
							Assert.True(e.EntryType == "OrderStatus");
						}

						if (e.Field == "Positions")
						{
							Assert.True(e.EntryType == "Position");

							Assert.True(e.IsArray);
							Assert.True(!e.IsOwnedType);
						}
					}
				}
			}
		}
	}
}