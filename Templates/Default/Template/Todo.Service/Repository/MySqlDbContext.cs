using Microsoft.EntityFrameworkCore;
using <#cs Write(Project.Namespace)#>.Service.Configuration;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

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
<#cs
foreach	(var definition in Definitions.Where(d => !(d.IsEnumeration
|| d.IsArray
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		modelBuilder.Entity<{definition.Name}Model>(entity =>");
	WriteLine("		{");
	WriteLine("			entity.HasKey(e => e.Id);");
	
	foreach	(var e in definition.Entries)
	{
		if (e.IsArray)
		{
			WriteLine($"			entity.OwnsMany(e => e.{e.Field}, e => {{ e.HasKey(e => e.Id); e.ToTable(\"{definition.Name}_{e.Field}\"); }});");
		}
		else if (e.IsOwnedType)
		{
			WriteLine($"			entity.OwnsOne(e => e.{e.Field}).ToTable(\"{definition.Name}_{e.Field}\");");
		}
	}
	
	WriteLine("		});");
}

Result = Result.TrimEnd();
#>
	}

	public DbSet<UserModel> Users { get; set; }
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsEnumeration
|| d.IsArray
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"	public DbSet<{definition.Name}Model> {definition.Name}s {{ get; set; }}");
}
#>}<#cs SetOutputPathAndSkipOtherDefinitions()#>