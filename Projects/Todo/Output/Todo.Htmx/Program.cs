using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using Todo.Client.Data;
using Todo.Client.Data.Identity;
using Todo.Htmx.LiveUpdates;
using Todo.Htmx.Identity;

namespace Todo.Htmx;

public sealed class PreserveEmptyStringMetadataProvider : IDisplayMetadataProvider
{
	public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
	{
		if (context.Key.ModelType == typeof(string))
		{
			context.DisplayMetadata.ConvertEmptyStringToNull = false;
		}
	}
}

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
				true);

		var map = new Najlot.Map.Map();
		builder.Services.AddSingleton(map.RegisterTodoClientDataMappings());

		// Configure Logging
		builder.Logging.ClearProviders();
		builder.Logging.AddNajlotLog(LogAdministrator.Instance);

		var dataServiceUrl = builder.Configuration.GetSection("DataServiceUrl")?.Get<string>() ?? throw new InvalidOperationException("DataServiceUrl not found.");

		builder.Services.AddHttpClient(Options.DefaultName, c =>
		{
			c.BaseAddress = new Uri(dataServiceUrl);
		});

		builder.Services
			.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.LoginPath = "/Identity/Login";
				options.LogoutPath = "/Identity/Logout";
				options.AccessDeniedPath = "/Identity/Login";
				options.ExpireTimeSpan = TimeSpan.FromDays(7);
				options.SlidingExpiration = true;
			});

		builder.Services.AddAuthorization();

		builder.Services.AddLocalization();
		builder.Services.Configure<MvcOptions>(options =>
		{
			options.ModelMetadataDetailsProviders.Add(new PreserveEmptyStringMetadataProvider());
		});
		builder.Services.AddControllersWithViews().AddViewLocalization();
		builder.Services.AddDistributedMemoryCache();

		builder.Services.AddSession(options =>
		{
			options.IdleTimeout = TimeSpan.FromDays(7);
			options.Cookie.HttpOnly = true;
			options.Cookie.IsEssential = true;
		});

		builder.Services.AddHttpContextAccessor();

		builder.Services.AddRazorPages();

		builder.Services.AddScoped<IUserDataStore, HttpContextUserDataStore>();
		builder.Services.AddSingleton<ILiveUpdateBridgeManager, LiveUpdateBridgeManager>();

		builder.Services.RegisterClientData();

		var app = builder.Build();
		var serviceProvider = app.Services;
		map.RegisterFactory(serviceProvider.GetRequiredService);

		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseSession();

		app.MapRazorPages();
		app.MapGet("/live-updates", LiveUpdatesEndpoint.HandleAsync).RequireAuthorization();

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

