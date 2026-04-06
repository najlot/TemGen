using <# Project.Namespace#>.Service.Features.Auth;
using <# Project.Namespace#>.Service.Features.GlobalSearch;
using <# Project.Namespace#>.Service.Features.History;
using <# Project.Namespace#>.Service.Features.Trash;
using <# Project.Namespace#>.Service.Features.Users;
using <# Project.Namespace#>.Service.Infrastructure.StorageBackup;
using <# Project.Namespace#>.Service.Shared.Realtime;
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))).OrderBy(d => d.Name))
{
	WriteLine($"using {Project.Namespace}.Service.Features.{definition.Name}s;");
}
#>

namespace <# Project.Namespace#>.Service;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		services.RegisterAuthFeature();
		services.RegisterUsersFeature();
		services.RegisterHistoryFeature();

<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))).OrderBy(d => d.Name))
{
	WriteLine($"\t\tservices.Register{definition.Name}Feature();");
}
	Write("\r\n");
#>		services.RegisterGlobalSearchFeature();
		services.RegisterTrashFeature();
		services.RegisterRealtimeServices();
		services.RegisterStorageBackupInfrastructure();

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>