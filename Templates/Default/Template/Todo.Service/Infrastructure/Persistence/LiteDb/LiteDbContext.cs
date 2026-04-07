using LiteDB;
using System.IO;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Features.Users;
using <# Project.Namespace#>.Service.Shared.Configuration;
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
|| d.IsArray
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))).OrderBy(d => d.Name))
{
	WriteLine($"using {Project.Namespace}.Service.Features.{definition.Name}s;");
}
#>

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.LiteDb;

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

		var bsonMapper = new BsonMapper();
		bsonMapper.RegisterType<string>
		(
			serialize: (s) => s ?? string.Empty,
			deserialize: (bson) => bson.IsNull ? string.Empty : bson.AsString
		);

		Database = new LiteDatabase(databasePath, bsonMapper);
	}

	public ILiteCollection<T> GetCollection<T>(string name)
	{
		return Database.GetCollection<T>(name);
	}

	public void EnsureCreated()
	{
		var historyCollection = GetCollection<HistoryModel>("HistoryEntries");
		historyCollection.EnsureIndex(item => item.Id, true);
		historyCollection.EnsureIndex(item => item.EntityId);
		historyCollection.EnsureIndex(item => item.TimeStamp);

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