using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using <# Project.Namespace#>.Service.Configuration;
using <# Project.Namespace#>.Service.Model;

namespace <# Project.Namespace#>.Service.Repository;

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
<#for definition in Definitions.Where(d => !(d.IsEnumeration
|| d.IsArray
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		modelBuilder.Entity<<# definition.Name#>Model>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasIndex(e => e.DeletedAt).IsUnique(false);
			<#for e in definition.Entries
#><#if e.IsArray
#>entity.OwnsMany(e => e.<# e.Field#>, e => { e.HasKey(e => e.Id); e.ToTable("<# definition.Name#>_<# e.Field#>"); });
			<#elseif e.IsOwnedType
#>entity.OwnsOne(e => e.<# e.Field#>).ToTable("<# definition.Name#>_<# e.Field#>");
			<#end#><#end#>
		});
<#end#>
	}

	public DbSet<UserModel> Users { get; set; }
<#for definition in Definitions.Where(d => !(d.IsEnumeration
|| d.IsArray
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>	public DbSet<<# definition.Name#>Model> <# definition.Name#>s { get; set; }
<#end#>}<#cs SetOutputPathAndSkipOtherDefinitions()#>