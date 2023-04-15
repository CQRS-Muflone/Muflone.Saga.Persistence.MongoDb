using MongoDB.Driver;
using Muflone.Saga.Persistence.MongoDb.Entities;
using System.Text.Json;

//using System.Text.Json;

namespace Muflone.Saga.Persistence.MongoDb
{
	public class MongoSagaStateRepository : ISagaRepository
	{
		private readonly IDbContext _dbContext;

		public MongoSagaStateRepository(IDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<TSagaState> GetByIdAsync<TSagaState>(Guid id) where TSagaState : class, new()
		{
			try
			{
				var filterBuilder = Builders<SagaState>.Filter;
				var filter = filterBuilder.And(
					filterBuilder.Eq(e => e.Id, id.ToString())
				);

				var sagaStateCursor = _dbContext.SagaState.FindSync(filter);
				var sagaState = await sagaStateCursor.FirstOrDefaultAsync();

				var originalData = JsonSerializer.Deserialize<TSagaState>(sagaState.Data);

				return originalData;
			}
			catch (MongoCommandException ex)
			{
				throw;
			}
		}

		public async Task SaveAsync<TSagaState>(Guid correlationId, TSagaState sagaState) where TSagaState : class, new()
		{
			try
			{
				var filterBuilder = Builders<SagaState>.Filter;
				var filter = filterBuilder.And(
					filterBuilder.Eq(e => e.Id, correlationId.ToString())
				);

				var update = Builders<SagaState>.Update
					.Set(e => e.Data, JsonSerializer.Serialize(sagaState));
				var entity = await _dbContext.SagaState
				.FindOneAndUpdateAsync(filter, update)
					.ConfigureAwait(false);

				if (entity is null)
				{
					entity = new SagaState(correlationId.ToString(),
						typeof(TSagaState).Assembly.GetName().Name!,
						JsonSerializer.Serialize(sagaState),
						DateTime.UtcNow, DateTime.MinValue);

					await _dbContext.SagaState.InsertOneAsync(entity);
				}
			}
			catch (MongoCommandException ex)
			{
				throw;
			}
		}

		public async Task CompleteAsync(Guid correlationId)
		{
			try
			{
				var filterBuilder = Builders<SagaState>.Filter;
				var filter = filterBuilder.And(
					filterBuilder.Eq(e => e.Id, correlationId.ToString())
				);

				var update = Builders<SagaState>.Update
					.Set(e => e.SagaFinished, DateTime.UtcNow);
				var entity = await _dbContext.SagaState
					.FindOneAndUpdateAsync(filter, update)
					.ConfigureAwait(false);

				if (entity is null)
					throw new Exception("An error occured saving SagaState");
			}
			catch (MongoCommandException ex)
			{
				throw;
			}
		}
	}
}