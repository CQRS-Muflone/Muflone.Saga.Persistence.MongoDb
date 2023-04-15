using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Muflone.Saga.Persistence.MongoDb
{
	public class DbContext : IDbContext
	{
		public IMongoCollection<Entities.SagaState> SagaState { get; }

		static DbContext()
		{
			ConfigureMappings();
		}

		public DbContext(IMongoDatabase db)
		{
			if (db == null)
				throw new ArgumentNullException(nameof(db));

			SagaState = db.GetCollection<Entities.SagaState>("SagaState");
			BuildSagaStatesIndexes();
		}

		private void BuildSagaStatesIndexes()
		{
			var indexBuilder = Builders<Entities.SagaState>.IndexKeys;
			var indexKeys = indexBuilder.Combine(
				indexBuilder.Ascending(e => e.Id),
				indexBuilder.Ascending(e => e.Type)
			);
			var index = new CreateIndexModel<Entities.SagaState>(indexKeys, new CreateIndexOptions()
			{
				Unique = true,
				Name = "ix_correlation_type"
			});
			SagaState.Indexes.CreateOne(index);
		}

		private static void ConfigureMappings()
		{
			BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
			try
			{
				BsonClassMap.RegisterClassMap<Entities.SagaState>(mapper =>
				{
					mapper.MapIdProperty(c => c.Id);
					mapper.MapProperty(c => c.Type);
					mapper.MapProperty(c => c.Data);
					mapper.MapProperty(c => c.SagaStarted).SetDefaultValue(() => DateTime.MinValue);
					mapper.MapProperty(c => c.SagaFinished).SetDefaultValue(() => DateTime.MinValue);
					mapper.MapCreator(s =>
						new Entities.SagaState(s.Id, s.Type, s.Data, s.SagaStarted, s.SagaFinished));
				});
			}
			catch
			{
				// swallowing exception, in case another concurrent thread has already registered the mapping
			}
		}
	}
}