using Microsoft.EntityFrameworkCore;
using Todo.Service.Configuration;
using Todo.Service.Model;

namespace Todo.Service.Repository;

public class MySqlDbContext : DbContext
{
	private readonly MySqlConfiguration _configuration;

	public MySqlDbContext(MySqlConfiguration configuration)
	{
		_configuration = configuration;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = $"server={_configuration.Host};" +
			$"port={_configuration.Port};" +
			$"database={_configuration.Database};" +
			$"uid={_configuration.User};" +
			$"password={_configuration.Password}";

		var serverVersion = ServerVersion.AutoDetect(connectionString);
		optionsBuilder.UseMySql(connectionString, serverVersion);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<UserModel>(entity =>
		{
			entity.HasKey(e => e.Id);
		});
		modelBuilder.Entity<NoteModel>(entity =>
		{
			entity.HasKey(e => e.Id);
		});
		modelBuilder.Entity<TodoItemModel>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.OwnsMany(e => e.Checklist, e => { e.HasKey(e => e.Id); e.ToTable("TodoItem_Checklist"); });
		});
	}

	public DbSet<UserModel> Users { get; set; }
	public DbSet<NoteModel> Notes { get; set; }
	public DbSet<TodoItemModel> TodoItems { get; set; }
}