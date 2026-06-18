using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Services;
using <# Project.Namespace#>.Blazor.Identity;
using <# Project.Namespace#>.Client.Data;
using <# Project.Namespace#>.Client.Data.Identity;

namespace <# Project.Namespace#>.Blazor;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		var map = new Najlot.Map.Map();
		builder.Services.AddSingleton(map.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientDataMappings());

		var dataServiceUrl = builder.Configuration.GetSection("DataServiceUrl")?.Get<string>();
		if (string.IsNullOrWhiteSpace(dataServiceUrl))
		{
			var dataServiceUri = new UriBuilder(builder.HostEnvironment.BaseAddress)
			{
				Port = 5000,
			};

			dataServiceUrl = dataServiceUri.Uri.ToString();
		}

		builder.Services.AddHttpClient(Options.DefaultName, c =>
		{
			c.BaseAddress = new Uri(dataServiceUrl);
		});

		builder.Services.AddOptions();
		builder.Services.AddAuthorizationCore();
		builder.Services.AddLocalization();
		builder.Services.AddMudServices(config =>
		{
			config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
			config.SnackbarConfiguration.RequireInteraction = false;
			config.SnackbarConfiguration.PreventDuplicates = false;
			config.SnackbarConfiguration.NewestOnTop = true;
			config.SnackbarConfiguration.ShowCloseIcon = true;
			config.SnackbarConfiguration.VisibleStateDuration = 4000;
			config.SnackbarConfiguration.HideTransitionDuration = 200;
			config.SnackbarConfiguration.ShowTransitionDuration = 200;
			config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
		});

		builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationService>();
		builder.Services.AddScoped(c => (IAuthenticationService)c.GetRequiredService<AuthenticationStateProvider>());
		builder.Services.AddScoped<IUserDataStore, UserDataStore>();

		builder.Services.RegisterClientData();

		var host = builder.Build();
		var serviceProvider = host.Services;
		map.RegisterFactory(serviceProvider.GetRequiredService);

		await host.RunAsync();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>