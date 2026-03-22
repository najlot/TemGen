using LiteDB;
using System.IO;
using <# Project.Namespace#>.Service.Configuration;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository.LiteDbImpl;

public sealed class LiteDbContext : IDisposable
{
	public LiteDatabase Database { get; }

	public LiteDbContext(LiteDbConfiguration configuration)
	{
		var databasePath = Path.GetFullPath(configuration.DatabasePath);
		var directoryPath = Path.GetDirectoryName(databasePath);

		if (!string.IsNullOrWhiteSpace(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}

		Database = new LiteDatabase(databasePath);
	}

	public ILiteCollection<T> GetCollection<T>(string name)
	{
		return Database.GetCollection<T>(name);
	}

	public void EnsureCreated()
	{
		var usersCollection = GetCollection<UserModel>("Users");
		usersCollection.EnsureIndex(item => item.Id, true);
		usersCollection.EnsureIndex(item => item.DeletedAt);
<#for definition in Definitions.Where(d => !(d.IsEnumeration
|| d.IsArray
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))#>
		var <# definition.NameLow#>Collection = GetCollection<<# definition.Name#>Model>("<# definition.Name#>s");
		<# definition.NameLow#>Collection.EnsureIndex(item => item.Id, true);
		<# definition.NameLow#>Collection.EnsureIndex(item => item.DeletedAt);
<#end#>	}

	public void Dispose()
	{
		Database.Dispose();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>