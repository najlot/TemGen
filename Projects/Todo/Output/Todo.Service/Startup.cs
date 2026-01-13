using Cosei.Service.Base;
using Cosei.Service.Http;
using Cosei.Service.RabbitMq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Todo.Service.Configuration;
using Todo.Service.Repository;
using Todo.Service.Services;
using Todo.Service.Mappings;

namespace Todo.Service;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		var rmqConfig = Configuration.ReadConfiguration<RabbitMqConfiguration>();
		var fileConfig = Configuration.ReadConfiguration<FileConfiguration>();
		var mysqlConfig = Configuration.ReadConfiguration<MySqlConfiguration>();
		var mongoDbConfig = Configuration.ReadConfiguration<MongoDbConfiguration>();
		var serviceConfig = Configuration.ReadConfiguration<ServiceConfiguration>();

		if (string.IsNullOrWhiteSpace(serviceConfig?.Secret))
		{
			throw new Exception($"Please set {nameof(ServiceConfiguration.Secret)} in the {nameof(ServiceConfiguration)}!");
		}

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

		if (rmqConfig != null)
		{
			rmqConfig.QueueName = "Todo.Service";
			services.AddCoseiRabbitMq(rmqConfig);
		}

		services.RegisterDataMappings();

		services.AddCoseiHttp();

		services.AddScoped<IUserService, UserService>();
		services.AddScoped<NoteService>();
		services.AddScoped<TodoItemService>();
		services.AddScoped<TokenService>();
		services.AddSignalR();

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
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app)
	{
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
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
			endpoints.MapHub<CoseiHub>("/cosei");
		});

		app.UseCosei();

		using var scope = app.ApplicationServices.CreateScope();
		scope.ServiceProvider.GetService<MySqlDbContext>()?.Database?.EnsureCreated();
	}
}