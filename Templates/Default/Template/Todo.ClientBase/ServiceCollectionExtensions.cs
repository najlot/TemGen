using Microsoft.Extensions.DependencyInjection;
using System;
using <# Project.Namespace#>.ClientBase.ViewModel;

namespace <# Project.Namespace#>.ClientBase;

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

<#for definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddTransient<All<# definition.Name#>sViewModel>();
<#end#>
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddTransient<<# definition.Name#>ViewModel>();
<#end#>
		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>