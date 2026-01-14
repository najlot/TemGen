using Cosei.Service.Base;
using Cosei.Service.Http;
using Cosei.Service.RabbitMq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using <#cs Write(Project.Namespace)#>.Service.Configuration;
using <#cs Write(Project.Namespace)#>.Service.Repository;
using <#cs Write(Project.Namespace)#>.Service.Services;

namespace <#cs Write(Project.Namespace)#>.Service;

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
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			services.AddScoped<I{definition.Name}Repository, MongoDb{definition.Name}Repository>();");
}
#>		}
		else if (mysqlConfig != null)
		{
			services.AddSingleton(mysqlConfig);
			services.AddScoped<MySqlDbContext>();
			services.AddScoped<IUserRepository, MySqlUserRepository>();
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			services.AddScoped<I{definition.Name}Repository, MySql{definition.Name}Repository>();");
}
#>		}
		else
		{
			services.AddSingleton(fileConfig ?? new FileConfiguration());
			services.AddScoped<IUserRepository, FileUserRepository>();
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"			services.AddScoped<I{definition.Name}Repository, File{definition.Name}Repository>();");
}
#>		}

		if (rmqConfig != null)
		{
			rmqConfig.QueueName = "<#cs Write(Project.Namespace)#>.Service";
			services.AddCoseiRabbitMq(rmqConfig);
		}

		var map = new Najlot.Map.Map().Register<#cs Write(Project.Namespace.Replace(".", ""))#>ServiceMappings();
		services.AddSingleton(map);

		services.AddCoseiHttp();

		services.AddScoped<IUserService, UserService>();
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		services.AddScoped<{definition.Name}Service>();");
}
#>		services.AddScoped<TokenService>();
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
}<#cs SetOutputPathAndSkipOtherDefinitions()#>