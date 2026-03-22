using <# Project.Namespace#>.Service.Services.GlobalSearch;
using <# Project.Namespace#>.Service.Services.Trash;

namespace <# Project.Namespace#>.Service.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		services.AddHttpContextAccessor();
		services.AddScoped<IUserIdProvider, HttpContextUserIdProvider>();
		services.AddScoped<IPermissionQueryFilter, PermissionQueryFilter>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<TokenService>();

<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))).OrderBy(d => d.Name))
{
	WriteLine($"\t\tservices.AddScoped<{definition.Name}Service>();");
}
	Write("\t\t\r\n");
#>		services.AddScoped<GlobalSearchService>();
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name))
{
	WriteLine($"\t\tservices.AddScoped<IGlobalSearchSource, {definition.Name}GlobalSearchSource>();");
}
	Write("\r\n");
#>		services.AddScoped<TrashService>();
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name))
{
	WriteLine($"\t\tservices.AddScoped<ITrashSource, {definition.Name}TrashSource>();");
}
	Write("\r\n");
#>		services.AddSignalR();
		services.AddScoped<Publisher>();
		services.AddScoped<IPublisher>(serviceProvider => serviceProvider.GetRequiredService<Publisher>());
		services.AddScoped<IOutboxPublisher>(serviceProvider => serviceProvider.GetRequiredService<Publisher>());
		services.AddHostedService<StorageBackupHostedService>();

		return services;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>