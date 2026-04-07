using Microsoft.Extensions.DependencyInjection;
using Todo.Service.Infrastructure.Persistence;
using Todo.Service.Infrastructure.Persistence.File;
using Todo.Service.Infrastructure.Persistence.LiteDb;
using Todo.Service.Infrastructure.Persistence.MongoDb;
using Todo.Service.Infrastructure.Persistence.MySql;
using Todo.Service.Shared.Configuration;

namespace Todo.Service.Features.Notes.Persistence;

public static class NotePersistenceServiceCollectionExtensions
{
	public static IServiceCollection RegisterFileNotePersistence(this IServiceCollection services)
	{
		services.AddScoped<INoteRepository, FileNoteRepository>();
		services.AddScoped<IEntityRepository<NoteModel>, FileNoteRepository>();
		return services;
	}

	public static IServiceCollection RegisterFileNotePersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<INoteRepository>(serviceKey, static (serviceProvider, key) =>
			new FileNoteRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		services.AddKeyedScoped<IEntityRepository<NoteModel>>(serviceKey, static (serviceProvider, key) =>
			new FileNoteRepository(serviceProvider.GetRequiredKeyedService<FileConfiguration>(key)));
		return services;
	}

	public static IServiceCollection RegisterLiteDbNotePersistence(this IServiceCollection services)
	{
		services.AddScoped<INoteRepository, LiteDbNoteRepository>();
		services.AddScoped<IEntityRepository<NoteModel>, LiteDbNoteRepository>();
		return services;
	}

	public static IServiceCollection RegisterLiteDbNotePersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<INoteRepository>(serviceKey, static (serviceProvider, key) =>
			new LiteDbNoteRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<NoteModel>>(serviceKey, static (serviceProvider, key) =>
			new LiteDbNoteRepository(serviceProvider.GetRequiredKeyedService<LiteDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMongoDbNotePersistence(this IServiceCollection services)
	{
		services.AddScoped<INoteRepository, MongoDbNoteRepository>();
		services.AddScoped<IEntityRepository<NoteModel>, MongoDbNoteRepository>();
		return services;
	}

	public static IServiceCollection RegisterMongoDbNotePersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<INoteRepository>(serviceKey, static (serviceProvider, key) =>
			new MongoDbNoteRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<NoteModel>>(serviceKey, static (serviceProvider, key) =>
			new MongoDbNoteRepository(serviceProvider.GetRequiredKeyedService<MongoDbContext>(key)));
		return services;
	}

	public static IServiceCollection RegisterMySqlNotePersistence(this IServiceCollection services)
	{
		services.AddScoped<INoteRepository, MySqlNoteRepository>();
		services.AddScoped<IEntityRepository<NoteModel>, MySqlNoteRepository>();
		return services;
	}

	public static IServiceCollection RegisterMySqlNotePersistence(this IServiceCollection services, object serviceKey)
	{
		services.AddKeyedScoped<INoteRepository>(serviceKey, static (serviceProvider, key) =>
			new MySqlNoteRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		services.AddKeyedScoped<IEntityRepository<NoteModel>>(serviceKey, static (serviceProvider, key) =>
			new MySqlNoteRepository(serviceProvider.GetRequiredKeyedService<MySqlDbContext>(key)));
		return services;
	}
}
