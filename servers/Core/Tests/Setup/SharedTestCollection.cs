namespace Core.Tests.Setup;

[CollectionDefinition("Tests")]
public class SharedTestCollection : ICollectionFixture<TestWebAppFactory>;