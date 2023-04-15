using Microsoft.Extensions.DependencyInjection;

namespace Muflone.Saga.Persistence.MongoDb.Tests;

public class MongoSagaRepositoryTest
{
	private readonly IServiceProvider _serviceProvider;

	public MongoSagaRepositoryTest()
	{
		var services = new ServiceCollection();

		var options = new MongoSagaStateRepositoryOptions("mongodb://localhost:27017", "BrewUpSaga");
		services.AddMongoSagaStateRepository(options);

		_serviceProvider = services.BuildServiceProvider();
	}

	[Fact]
	public async Task Should_Save_SagaState()
	{
		var mongoSagaStateRepository = _serviceProvider.GetService<ISagaRepository>();

		var correlationId = Guid.NewGuid();
		await mongoSagaStateRepository.SaveAsync(correlationId, new TestSagaState
		{
			CorrelationId = correlationId,
			Name = "Test"
		});

		var sagaState = await mongoSagaStateRepository.GetByIdAsync<TestSagaState>(correlationId);

		Assert.NotNull(sagaState);
		Assert.Equal(correlationId, sagaState.CorrelationId);
	}

	public class TestSagaState
	{
		public Guid CorrelationId { get; set; }
		public string Name { get; set; }
	}
}