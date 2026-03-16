using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository;

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

		modelBuilder.Entity<UserModel>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasIndex(e => e.DeletedAt).IsUnique(false);
		});
		modelBuilder.Entity<NoteModel>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasIndex(e => e.DeletedAt).IsUnique(false);
		});
		modelBuilder.Entity<TodoItemModel>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasIndex(e => e.DeletedAt).IsUnique(false);
			entity.OwnsMany(e => e.Checklist, e => { e.HasKey(e => e.Id); e.ToTable("TodoItem_Checklist"); });
		});
	}

	public DbSet<UserModel> Users { get; set; }
	public DbSet<NoteModel> Notes { get; set; }
	public DbSet<TodoItemModel> TodoItems { get; set; }
}