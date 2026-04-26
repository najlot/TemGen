using LiteDB;
using System.IO;
using Todo.Service.Features.Filters;
using Todo.Service.Features.History;
using Todo.Service.Features.Users;
using Todo.Service.Shared.Configuration;
using Todo.Service.Features.Notes;
using Todo.Service.Features.TodoItems;


namespace Todo.Service.Infrastructure.Persistence.LiteDb;

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
		var filtersCollection = GetCollection<FilterModel>("Filters");
		filtersCollection.EnsureIndex(item => item.Id, true);
		filtersCollection.EnsureIndex(item => item.UserId);
		filtersCollection.EnsureIndex(item => item.TargetType);

		var historyCollection = GetCollection<HistoryModel>("HistoryEntries");
		historyCollection.EnsureIndex(item => item.Id, true);
		historyCollection.EnsureIndex(item => item.EntityId);
		historyCollection.EnsureIndex(item => item.TimeStamp);

		var usersCollection = GetCollection<UserModel>("Users");
		usersCollection.EnsureIndex(item => item.Id, true);
		usersCollection.EnsureIndex(item => item.DeletedAt);

		var noteCollection = GetCollection<NoteModel>("Notes");
		noteCollection.EnsureIndex(item => item.Id, true);
		noteCollection.EnsureIndex(item => item.DeletedAt);

		var todoItemCollection = GetCollection<TodoItemModel>("TodoItems");
		todoItemCollection.EnsureIndex(item => item.Id, true);
		todoItemCollection.EnsureIndex(item => item.DeletedAt);
	}

	public void Dispose()
	{
		Database.Dispose();
	}
}