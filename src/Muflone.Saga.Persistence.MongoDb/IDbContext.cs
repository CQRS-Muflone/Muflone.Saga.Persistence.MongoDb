using MongoDB.Driver;

namespace Muflone.Saga.Persistence.MongoDb
{
	public interface IDbContext
	{
		IMongoCollection<Entities.SagaState> SagaState { get; }
	}
}