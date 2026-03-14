using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using <#cs Write(Project.Namespace)#>.Service.Configuration;
using <#cs Write(Project.Namespace)#>.Service.Model;

namespace <#cs Write(Project.Namespace)#>.Service.Repository;

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
<#cs
foreach	(var definition in Definitions.Where(d => !(d.IsEnumeration
|| d.IsArray
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		modelBuilder.Entity<{definition.Name}Model>(entity =>");
	WriteLine("		{");
	WriteLine("			entity.HasKey(e => e.Id);");
	WriteLine("			entity.HasIndex(e => e.DeletedAt).IsUnique(false);");
	
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