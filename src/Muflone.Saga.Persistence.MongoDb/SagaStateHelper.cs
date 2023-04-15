using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Muflone.Saga.Persistence.MongoDb;

public static class SagaStateHelper
{
	public static IServiceCollection AddMongoSagaStateRepository(this IServiceCollection services,
		MongoSagaStateRepositoryOptions options)
	{
		services.AddSingleton<IMongoClient>(_ => new MongoClient(options.ConnectionString));
		services.AddSingleton(provider =>
			provider.GetService<IMongoClient>()
				?.GetDatabase(options.DatabaseName)
				.WithWriteConcern(WriteConcern.W1));

		services.AddSingleton<IDbContext, DbContext>();
		services.AddSingleton<ISagaRepository, MongoSagaStateRepository>();

		return services;
	}
}