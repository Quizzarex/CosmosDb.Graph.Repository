using Xunit;
using System;
using Microsoft.Azure.Documents.Client;

using CosmosDb.Graph;

namespace CosmosDb.Graph.Tests
{
    public class CosmosDbConnectionTests : IDisposable
    {
        private readonly string _databaseIdentifier;
        private readonly string _collectionIdentifier;
        private CosmosDbConnection _sut;
        
        public CosmosDbConnectionTests()
        {
            _databaseIdentifier = Guid.NewGuid().ToString();
            _collectionIdentifier = Guid.NewGuid().ToString();
            _sut = new CosmosDbConnection();
        }

        [Fact]
        public void CosmosDbConnection__BeforeCreatingConnectionWithClientAndCollection__AssertNull()
        {
            Assert.Null(_sut.Client);
            Assert.Null(_sut.Collection);
        }

        [Fact]
        public void CosmosDbConnection__AfterCreatingConnectionWithClientAndCollection__AssertNotNull()
        {
            _sut = CosmosDbConnection.CreateCosmosDbConnection(
                "https://localhost:8081",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                _databaseIdentifier,
                _collectionIdentifier,
                throughput: 400,
                connectionMode: ConnectionMode.Gateway,
                protocol: Protocol.Tcp
            );

            Assert.NotNull(_sut.Client);
            Assert.NotNull(_sut.Collection);
        }

        public void Dispose()
        {
            if (_sut.Client != null)
                _sut.Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseIdentifier)).Wait();
            
            _sut.Dispose();
        }
    }
}