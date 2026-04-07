using Microsoft.Extensions.DependencyInjection;

namespace <# Project.Namespace#>.Service.Features.<# Definition.Name#>s;

public static class <# Definition.Name#>ServiceCollectionExtensions
{
	public static IServiceCollection Register<# Definition.Name#>Feature(this IServiceCollection services)
	{
		services.AddScoped<<# Definition.Name#>Service>();
		return services;
	}
}
<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>