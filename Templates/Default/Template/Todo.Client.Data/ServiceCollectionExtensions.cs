using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Client.Data.Filters;
using <# Project.Namespace#>.Client.Data.GlobalSearch;
using <# Project.Namespace#>.Client.Data.History;
using <# Project.Namespace#>.Client.Data.Identity;
<#cs
foreach (var contractNamespace in Definitions
	.Where(d => !d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
	.Select(definition => 
	{
		if (definition.IsEnumeration || definition.IsOwnedType || definition.IsArray)
		{
			return $"{Project.Namespace}.Client.Data.{GetChildFeatureFolderName(definition.Name)}";
		}
		
		return $"{Project.Namespace}.Client.Data.{definition.Name}s";
	})
	.Distinct()
	.OrderBy(contractNamespace => contractNamespace))
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
			serviceCollection.AddScoped<IFilterService, FilterService>();
			serviceCollection.AddScoped<IGlobalSearchService, GlobalSearchService>();
			serviceCollection.AddScoped<IHistoryService, HistoryService>();
			serviceCollection.AddScoped<ITrashService, TrashService>();
<#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddScoped<I<# definition.Name#>Service, <# definition.Name#>Service>();
<#end#>			serviceCollection.AddScoped<IUserService, UserService>();
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>