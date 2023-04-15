namespace Muflone.Saga.Persistence.MongoDb.Entities
{
	public record SagaState(string Id, string Type, string Data, DateTime SagaStarted, DateTime SagaFinished);
}