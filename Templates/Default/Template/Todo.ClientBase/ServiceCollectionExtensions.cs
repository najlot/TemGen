using Microsoft.Extensions.DependencyInjection;
using System;
using <# Project.Namespace#>.ClientBase.GlobalSearch;
using <# Project.Namespace#>.ClientBase.History;
using <# Project.Namespace#>.ClientBase.Identity;
using <# Project.Namespace#>.ClientBase.Shared;
<#cs 
var features = Definitions
	.Where(e => !(e.IsArray 
				|| e.IsEnumeration 
				|| e.IsOwnedType 
				|| e.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
	.Distinct()
	.OrderByDescending(d => d.Name);

foreach (var feature in features)
{
	WriteLine($"using {Project.Namespace}.ClientBase.{feature.Name}s;");
}
#>using <# Project.Namespace#>.ClientBase.Trash;

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
			serviceCollection.AddTransient<GlobalSearchViewModel>();
			serviceCollection.AddTransient<EntityHistoryViewModel>();
			serviceCollection.AddTransient<TrashViewModel>();

<#for definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddTransient<<# definition.Name#>sViewModel>();
<#end#>
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>			serviceCollection.AddTransient<<# definition.Name#>ViewModel>();
<#end
#>		}
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>