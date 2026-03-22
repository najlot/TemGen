using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using <# Project.Namespace#>.Service.Repository.LiteDbImpl;
using <# Project.Namespace#>.Service.Repository.MySqlImpl;
using <# Project.Namespace#>.Service.Services;

namespace <# Project.Namespace#>.Service;

public static class WebApplicationExtensions
{
	public static WebApplication ConfigureApplicationPipeline(this WebApplication app)
	{
		app.UseSwagger();
		app.UseSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
			options.RoutePrefix = "swagger";
		});

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
		app.UseMiddleware<OutboxPublisherMiddleware>();
		app.MapHealthChecks("/health");
		app.MapHealthChecks("/health/live", new HealthCheckOptions
		{
			Predicate = registration => registration.Tags.Contains("live")
		});
		app.MapHealthChecks("/health/ready", new HealthCheckOptions
		{
			Predicate = registration => registration.Tags.Contains("ready")
		});
		app.MapControllers();
		app.MapHub<MessageHub>("/events");

		return app;
	}

	public static WebApplication EnsureStorageCreated(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		if (scope.ServiceProvider.GetService<LiteDbContext>() is { } liteDbContext)
		{
			liteDbContext.EnsureCreated();
		}

		if (scope.ServiceProvider.GetService<MySqlDbContext>() is { } dbc)
		{
			dbc.Database.EnsureCreated();
			// Use EF migrations
			// dotnet ef migrations add NameOfTheMigration
			// dotnet ef database update
			// dbc.Database.Migrate();
		}

		return app;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>