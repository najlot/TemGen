using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Destinations;
using Najlot.Log.Extensions.Logging;
using Najlot.Log.Middleware;
using System;
using System.IO;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Service.Configuration;
using <#cs Write(Project.Namespace)#>.Service.Repository;
using <#cs Write(Project.Namespace)#>.Service.Services;

namespace <#cs Write(Project.Namespace)#>.Service;

internal static class Program
{
	private static void LogErrorOccured(object? sender, LogErrorEventArgs e)
	{
		Console.WriteLine(e.Message + Environment.NewLine + e.Exception);
	}

	public static async Task Main(string[] args)
	{
		LogErrorHandler.Instance.ErrorOccured += LogErrorOccured;

		var builder = WebApplication.CreateBuilder(args);
		var configuration = builder.Configuration;

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

		builder.Logging.ClearProviders();
		builder.Logging.AddNajlotLog(LogAdministrator.Instance);

		var fileConfigFound = configuration.TryReadConfiguration<FileConfiguration>(out var fileConfig);
		var mongoDbConfigFound = configuration.TryReadConfiguration<MongoDbConfiguration>(out var mongoDbConfig);
		var mysqlConfigFound = configuration.TryReadConfiguration<MySqlConfiguration>(out var mysqlConfig);
		var serviceConfigFound = configuration.TryReadConfiguration<ServiceConfiguration>(out var serviceConfig);

		if (!(fileConfigFound || mongoDbConfigFound || mysqlConfigFound))
		{
			ConfigurationReader.WriteConfigurationExample<FileConfiguration>();
			ConfigurationReader.WriteConfigurationExample<MongoDbConfiguration>();
			ConfigurationReader.WriteConfigurationExample<MySqlConfiguration>();
		}

		if (!serviceConfigFound) ConfigurationReader.WriteConfigurationExample<ServiceConfiguration>();

		if (string.IsNullOrWhiteSpace(serviceConfig?.Secret))
		{
			throw new Exception($"Please set {nameof(ServiceConfiguration.Secret)} in the {nameof(ServiceConfiguration)}!");
		}

		var services = builder.Services;
		services.AddSingleton(serviceConfig);

		if (mongoDbConfig != null)
		{
			services.AddSingleton(mongoDbConfig);
			services.AddSingleton<MongoDbContext>();
			services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();
			services.AddScoped<IUserRepository, MongoDbUserRepository>();
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			services.AddScoped<I{definition.Name}Repository, MongoDb{definition.Name}Repository>();");
}

Result = Result.TrimEnd();
#>
		}
		else if (mysqlConfig != null)
		{
			services.AddSingleton(mysqlConfig);
			services.AddScoped<MySqlDbContext>();
			services.AddScoped<IUnitOfWork, MySqlUnitOfWork>();
			services.AddScoped<IUserRepository, MySqlUserRepository>();
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			services.AddScoped<I{definition.Name}Repository, MySql{definition.Name}Repository>();");
}

Result = Result.TrimEnd();
#>
		}
		else
		{
			services.AddSingleton(fileConfig ?? new FileConfiguration());
			services.AddScoped<IUnitOfWork, FileUnitOfWork>();
			services.AddScoped<IUserRepository, FileUserRepository>();
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			services.AddScoped<I{definition.Name}Repository, File{definition.Name}Repository>();");
}

Result = Result.TrimEnd();
#>
		}

		var map = new Najlot.Map.Map().Register<#cs Write(Project.Namespace)#>ServiceMappings();
		services.AddSingleton(map);

		services.AddScoped<IUserService, UserService>();
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		services.AddScoped<{definition.Name}Service>();");
}
#>		services.AddScoped<TokenService>();

		services.AddSignalR();
		services.AddSingleton<IPublisher, Publisher>();

		var validationParameters = TokenService.GetValidationParameters(serviceConfig.Secret);

		services.AddAuthentication(x =>
		{
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(x =>
		{
			x.RequireHttpsMetadata = false;
			x.TokenValidationParameters = validationParameters;
			x.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];
					// Blazor WebAssembly doesn't support sending the JWT in the Authorization header when connecting to a SignalR hub,
					// so we need to check the query string for the token when the request is for the hub.
					// If the request is for on of our SignalR hub ...
					if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/events"))
					{
						// Take the token from the query string
						context.Token = accessToken;
					}
					return Task.CompletedTask;
				}
			};
		});

		services.AddControllers();

		var app = builder.Build();

		app.UseCors(c =>
		{
			c.AllowAnyOrigin();
			c.AllowAnyMethod();
			c.AllowAnyHeader();
		});

		app.UseAuthentication();
		// app.UseHttpsRedirection();
		app.UseRouting();
		app.UseAuthorization();
		app.MapControllers();
		app.MapHub<MessageHub>("/events");

		try
		{
			using var scope = app.Services.CreateScope();
			var serviceProvider = scope.ServiceProvider;
			if (serviceProvider.GetService<MySqlDbContext>() is { } dbc)
			{
				dbc.Database.EnsureCreated();
				// Use EF migrations
				// dotnet ef migrations add NameOfTheMigration
				// dotnet ef database update
				// dbc.Database.Migrate();
			}

			await app.RunAsync();
		}
		catch (Exception ex)
		{
			var log = LogAdministrator.Instance.GetLogger(typeof(Program));
			log.Error(ex, "An unhandled exception occurred while running the application.");
			LogAdministrator.Instance.Flush();
		}

		LogAdministrator.Instance.Dispose();
		LogErrorHandler.Instance.ErrorOccured -= LogErrorOccured;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>