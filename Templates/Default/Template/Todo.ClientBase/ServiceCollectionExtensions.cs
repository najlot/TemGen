using Microsoft.Extensions.DependencyInjection;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;
using System;

namespace <#cs Write(Project.Namespace)#>.ClientBase;

public static class ServiceCollectionExtensions
{
	public static void RegisterViewModels(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<LoginViewModel>();
		serviceCollection.AddTransient<ProfileViewModel>();
		serviceCollection.AddSingleton<Func<ProfileViewModel>>(c => () => c.GetRequiredService<ProfileViewModel>());
		serviceCollection.AddTransient<LoginProfileViewModel>();
		serviceCollection.AddSingleton<Func<LoginProfileViewModel>>(c => () => c.GetRequiredService<LoginProfileViewModel>());
		serviceCollection.AddScoped<MenuViewModel>();

<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		serviceCollection.AddScoped<All{definition.Name}sViewModel>();");
}

WriteLine("");

foreach(var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		serviceCollection.AddTransient<{definition.Name}ViewModel>();");
	WriteLine($"		serviceCollection.AddSingleton<Func<{definition.Name}ViewModel>>(c => () => c.GetRequiredService<{definition.Name}ViewModel>());");
}

Result = Result.TrimEnd();
#>
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>