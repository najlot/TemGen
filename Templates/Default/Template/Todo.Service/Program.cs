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
		var configPath = Path.Combine("config", "Log.config");
		configPath = Path.GetFullPath(configPath);

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

		builder.Logging.ClearProviders();
		builder.Logging.AddNajlotLog(LogAdministrator.Instance);

		var configuration = builder.Configuration;
		var fileConfig = configuration.ReadConfiguration<FileConfiguration>();
		var mysqlConfig = configuration.ReadConfiguration<MySqlConfiguration>();
		var mongoDbConfig = configuration.ReadConfiguration<MongoDbConfiguration>();
		var serviceConfig = configuration.ReadConfiguration<ServiceConfiguration>();

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

		using (var scope = app.Services.CreateScope())
		{
			scope.ServiceProvider.GetService<MySqlDbContext>()?.Database?.EnsureCreated();
		}

		await app.RunAsync();

		LogAdministrator.Instance.Dispose();
		LogErrorHandler.Instance.ErrorOccured -= LogErrorOccured;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>