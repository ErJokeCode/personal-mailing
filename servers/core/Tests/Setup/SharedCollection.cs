namespace Core.Tests.Setup;

[CollectionDefinition(nameof(SharedCollection))]
public class SharedCollection : ICollectionFixture<ApplicationFactory>;