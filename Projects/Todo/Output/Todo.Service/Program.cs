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
using Todo.Service.Configuration;
using Todo.Service.Repository;
using Todo.Service.Services;

namespace Todo.Service;

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
			services.AddScoped<INoteRepository, MongoDbNoteRepository>();
			services.AddScoped<ITodoItemRepository, MongoDbTodoItemRepository>();
		}
		else if (mysqlConfig != null)
		{
			services.AddSingleton(mysqlConfig);
			services.AddScoped<MySqlDbContext>();
			services.AddScoped<IUserRepository, MySqlUserRepository>();
			services.AddScoped<INoteRepository, MySqlNoteRepository>();
			services.AddScoped<ITodoItemRepository, MySqlTodoItemRepository>();
		}
		else
		{
			services.AddSingleton(fileConfig ?? new FileConfiguration());
			services.AddScoped<IUserRepository, FileUserRepository>();
			services.AddScoped<INoteRepository, FileNoteRepository>();
			services.AddScoped<ITodoItemRepository, FileTodoItemRepository>();
		}

		var map = new Najlot.Map.Map().RegisterTodoServiceMappings();
		services.AddSingleton(map);

		services.AddScoped<IUserService, UserService>();
		services.AddScoped<NoteService>();
		services.AddScoped<TodoItemService>();
		services.AddScoped<TokenService>();

		services.AddSignalR();
		services.AddSingleton<MessageHub>();
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
