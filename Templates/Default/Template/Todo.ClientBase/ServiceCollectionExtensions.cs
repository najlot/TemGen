using Microsoft.Extensions.DependencyInjection;
using System;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

namespace <#cs Write(Project.Namespace)#>.ClientBase;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection serviceCollection)
	{
		public void RegisterClientBase()
		{
			serviceCollection.RegisterClientBaseViewModels();
		}

		public void RegisterClientBaseViewModels()
		{
			serviceCollection.AddTransient(typeof(ViewModelBaseParameters<>));

			serviceCollection.AddTransient<LoginViewModel>();
			serviceCollection.AddTransient<RegisterViewModel>();
			serviceCollection.AddTransient<MenuViewModel>();
			serviceCollection.AddTransient<ManageViewModel>();

<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			serviceCollection.AddTransient<All{definition.Name}sViewModel>();");
}

WriteLine("");

foreach(var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			serviceCollection.AddTransient<{definition.Name}ViewModel>();");
}

Result = Result.TrimEnd();
#>
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>