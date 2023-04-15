using System.Diagnostics.CodeAnalysis;

namespace Muflone.Saga.Persistence.MongoDb;

[ExcludeFromCodeCoverage]
public record MongoSagaStateRepositoryOptions(string ConnectionString, string DatabaseName);