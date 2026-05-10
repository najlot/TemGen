using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Client.Data.Favorites;
using <# Project.Namespace#>.Client.Data.Filters;
using <# Project.Namespace#>.Client.Data.GlobalSearch;
using <# Project.Namespace#>.Client.Data.History;
using <# Project.Namespace#>.Client.Data.Identity;
<#cs
foreach (var contractNamespace in Definitions
	.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
	.Select(definition => $"{Project.Namespace}.Client.Data.{FixPathPluralization(definition.Name + "s")}")

	.Distinct()
	.Order())
{
	WriteLine($"using {contractNamespace};");
}
#>using <# Project.Namespace#>.Client.Data.Trash;
using <# Project.Namespace#>.Client.Data.Users;

namespace <# Project.Namespace#>.Client.Data;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection serviceCollection)
	{
		public void RegisterClientData()
		{
			serviceCollection.RegisterClientDataIdentity();
			serviceCollection.RegisterClientDataRepositories();
			serviceCollection.RegisterClientDataServices();
		}

		public void RegisterClientDataIdentity()
		{
			serviceCollection.AddScoped<IPasswordResetService, PasswordResetService>();
			serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
			serviceCollection.AddScoped<ITokenProvider, RefreshingTokenProvider>();
			serviceCollection.AddScoped<ITokenService, TokenService>();
		}

		public void RegisterClientDataRepositories()
		{
			serviceCollection.AddScoped<IFavoriteRepository, FavoriteRepository>();
			serviceCollection.AddScoped<IFilterRepository, FilterRepository>();
			serviceCollection.AddScoped<IGlobalSearchRepository, GlobalSearchRepository>();
			serviceCollection.AddScoped<IHistoryRepository, HistoryRepository>();
			serviceCollection.AddScoped<ITrashRepository, TrashRepository>();
<#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddScoped<I<# definition.Name#>Repository, <# definition.Name#>Repository>();
<#end#>			serviceCollection.AddScoped<IUserRepository, UserRepository>();
		}

		public void RegisterClientDataServices()
		{
			serviceCollection.AddScoped<IApiEventConnectionProvider, ApiEventConnectionProvider>();
			serviceCollection.AddScoped<IFavoriteService, FavoriteService>();
			serviceCollection.AddScoped<IFilterService, FilterService>();
			serviceCollection.AddScoped<IGlobalSearchService, GlobalSearchService>();

<#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddScoped<IEntityHistoryLocalizer, <# definition.Name#>HistoryLocalizer>();
<#end#>			serviceCollection.AddScoped<IHistoryService, HistoryService>();
			serviceCollection.AddScoped<ITrashService, TrashService>();
<#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddScoped<I<# definition.Name#>Service, <# definition.Name#>Service>();
<#end#>			serviceCollection.AddScoped<IUserService, UserService>();
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>