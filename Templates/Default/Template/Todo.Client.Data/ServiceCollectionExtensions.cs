using Microsoft.Extensions.DependencyInjection;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.Data.Repositories;
using <# Project.Namespace#>.Client.Data.Repositories.Implementation;
using <# Project.Namespace#>.Client.Data.Services;
using <# Project.Namespace#>.Client.Data.Services.Implementation;

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
			serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
			serviceCollection.AddScoped<ITokenProvider, RefreshingTokenProvider>();
			serviceCollection.AddScoped<ITokenService, TokenService>();
		}

		public void RegisterClientDataRepositories()
		{
<#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddScoped<I<# definition.Name#>Repository, <# definition.Name#>Repository>();
<#end#>			serviceCollection.AddScoped<IUserRepository, UserRepository>();
		}

		public void RegisterClientDataServices()
		{
<#for definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddScoped<I<# definition.Name#>Service, <# definition.Name#>Service>();
<#end#>			serviceCollection.AddScoped<IUserService, UserService>();
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>