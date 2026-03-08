using Microsoft.Extensions.DependencyInjection;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Client.Data.Repositories;
using <#cs Write(Project.Namespace)#>.Client.Data.Repositories.Implementation;
using <#cs Write(Project.Namespace)#>.Client.Data.Services;
using <#cs Write(Project.Namespace)#>.Client.Data.Services.Implementation;

namespace <#cs Write(Project.Namespace)#>.Client.Data;

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
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			serviceCollection.AddScoped<I{definition.Name}Repository, {definition.Name}Repository>();");
}
#>			serviceCollection.AddScoped<IUserRepository, UserRepository>();
		}

		public void RegisterClientDataServices()
		{
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray || d.IsEnumeration || d.IsOwnedType || d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			serviceCollection.AddScoped<I{definition.Name}Service, {definition.Name}Service>();");
}
#>			serviceCollection.AddScoped<IUserService, UserService>();
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>