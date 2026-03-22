using LiteDB;
using System.IO;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository.LiteDbImpl;

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