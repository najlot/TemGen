using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
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

namespace <# Project.Namespace#>.Service.Infrastructure.Persistence.MySql;

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

	public DbSet<HistoryModel> HistoryEntries { get; set; }
	public DbSet<UserModel> Users { get; set; }
<#for definition in Definitions.Where(d => !(d.IsEnumeration
|| d.IsArray
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>	public DbSet<<# definition.Name#>Model> <# definition.Name#>s { get; set; }
<#end#>}<#cs SetOutputPathAndSkipOtherDefinitions()#>