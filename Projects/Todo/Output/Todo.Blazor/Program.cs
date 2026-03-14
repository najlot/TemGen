using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Todo.Blazor.Identity;
using Todo.Client.Data;
using Todo.Client.Data.Identity;

namespace Todo.Blazor;

public class Program
{
	private static void LogErrorOccured(object? sender, LogErrorEventArgs e)
	{
		Console.WriteLine(e.Message + Environment.NewLine + e.Exception);
	}

	public static void Main(string[] args)
	{
		LogErrorHandler.Instance.ErrorOccured += LogErrorOccured;

		var builder = WebApplication.CreateBuilder(args);

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
			/*.ReadConfiguration(builder.Configuration)*/;

		var map = new Najlot.Map.Map();
		builder.Services.AddSingleton(map.RegisterTodoClientDataMappings());

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

		builder.Services.AddScoped<IUserDataStore, UserDataStore>();

		builder.Services.RegisterClientData();

		var app = builder.Build();
		var serviceProvider = app.Services;
		map.RegisterFactory(serviceProvider.GetRequiredService);

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
}