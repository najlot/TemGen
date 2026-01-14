using Cosei.Client.Base;
using Cosei.Client.Http;
using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using <#cs Write(Project.Namespace)#>.Blazor.Identity;
using <#cs Write(Project.Namespace)#>.Blazor.Services;
using <#cs Write(Project.Namespace)#>.Blazor.Services.Implementation;
using <#cs Write(Project.Namespace)#>.Client.Data;
using <#cs Write(Project.Namespace)#>.Client.Data.Identity;
using <#cs Write(Project.Namespace)#>.Client.Data.Repositories;
using <#cs Write(Project.Namespace)#>.Client.Data.Repositories.Implementation;
using <#cs Write(Project.Namespace)#>.Client.Data.Services;
using <#cs Write(Project.Namespace)#>.Client.Data.Services.Implementation;

namespace <#cs Write(Project.Namespace)#>.Blazor;

public class Program
{
	private static void LogErrorOccured(object? sender, LogErrorEventArgs e)
	{
		Console.WriteLine(e.Message + Environment.NewLine + e.Exception);
	}

	public static void Main(string[] args)
	{
		var configFolderPath = Path.GetFullPath("config");
		var configPath = Path.Combine(configFolderPath, "Log.config");
		configPath = Path.GetFullPath(configPath);
		Directory.CreateDirectory(configFolderPath);

		LogErrorHandler.Instance.ErrorOccured += LogErrorOccured;

		LogAdministrator.Instance
			.SetLogLevel(Najlot.Log.LogLevel.Debug)
			.SetCollectMiddleware<ConcurrentCollectMiddleware, FileDestination>()
			.SetCollectMiddleware<ConcurrentCollectMiddleware, ConsoleDestination>()
			.AddConsoleDestination(useColors: true)
			.AddFileDestination(
				Path.Combine("logs", "log.txt"),
				30,
				Path.Combine("logs", ".logs"),
				true)
			.ReadConfigurationFromXmlFile(configPath, true, true);

		var builder = WebApplication.CreateBuilder(args);
		var map = new Najlot.Map.Map();
		builder.Services.AddSingleton(map.Register<#cs Write(Project.Namespace.Replace(".", ""))#>ClientDataMappings());

		// Configure Logging
		builder.Logging.ClearProviders();
		builder.Logging.AddNajlotLog(LogAdministrator.Instance);

		// Add services to the container.
		var dataServiceUrl = builder.Configuration.GetSection("DataServiceUrl")?.Get<string>() ?? throw new InvalidOperationException("DataServiceUrl not found.");

		builder.Services.AddHttpClient(Options.DefaultName, c =>
		{
			c.BaseAddress = new Uri(dataServiceUrl);
		});

		builder.Services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
		builder.Services.AddAuthorization();

		builder.Services.AddLocalization();

		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();

		builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationService>();
		builder.Services.AddScoped(c => (IAuthenticationService)c.GetRequiredService<AuthenticationStateProvider>());

		builder.Services.AddScoped<IRequestClient, HttpFactoryRequestClient>();
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<ITokenProvider, RefreshingTokenProvider>();
		builder.Services.AddScoped<IUserDataStore, UserDataStore>();

		builder.Services.AddScoped<IRegistrationService, RegistrationService>();

		builder.Services.AddScoped<IUserRepository, UserRepository>();
		builder.Services.AddScoped<IUserService, UserService>();
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray 
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		builder.Services.AddScoped<I{definition.Name}Repository, {definition.Name}Repository>();");
}

WriteLine("");

foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		builder.Services.AddScoped<I{definition.Name}Service, {definition.Name}Service>();");
}
#>
		builder.Services.AddScoped<ISubscriberProvider, SubscriberProvider>();

		var app = builder.Build();
		var serviceProvider = app.Services;
		map.RegisterFactory(t =>
		{
			if (t.GetConstructor(Type.EmptyTypes) is not null)
			{
				return Activator.CreateInstance(t) ?? throw new NullReferenceException($"Could not create {t.FullName}. Result is null.");
			}

			return serviceProvider.GetRequiredService(t);
		});

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();
		app.MapRazorPages();
		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");

		var supportedCultures = new[] { "en", "de" };
		var localizationOptions = new RequestLocalizationOptions()
			.SetDefaultCulture(supportedCultures[0])
			.AddSupportedCultures(supportedCultures)
			.AddSupportedUICultures(supportedCultures);

		app.UseRequestLocalization(localizationOptions);

		app.Run();

		LogAdministrator.Instance.Dispose();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>