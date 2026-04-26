using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Todo.Service.Features.Filters;
using Todo.Service.Features.History;
using Todo.Service.Features.Users;
using Todo.Service.Shared.Configuration;
using Todo.Service.Features.Notes;
using Todo.Service.Features.TodoItems;


namespace Todo.Service.Infrastructure.Persistence.MySql;

public class MySqlDbContext(
	MySqlConfiguration configuration,
	ILoggerFactory loggerFactory) : DbContext
{
	private static readonly ConcurrentDictionary<string, ServerVersion> _knownVersions = new();

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = $"server={configuration.Host};" +
			$"port={configuration.Port};" +
			$"database={configuration.Database};" +
			$"uid={configuration.User};" +
			$"password={configuration.Password}";

		if (!_knownVersions.TryGetValue(connectionString, out var version))
		{
			version = ServerVersion.AutoDetect(connectionString);
			_knownVersions[connectionString] = version;
		}

		optionsBuilder.UseMySql(connectionString, version);
		optionsBuilder.UseLoggerFactory(loggerFactory);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(MySqlDbContext).Assembly);
	}

	public DbSet<FilterModel> Filters { get; set; }
	public DbSet<HistoryModel> HistoryEntries { get; set; }
	public DbSet<UserModel> Users { get; set; }
	public DbSet<NoteModel> Notes { get; set; }
	public DbSet<TodoItemModel> TodoItems { get; set; }
}