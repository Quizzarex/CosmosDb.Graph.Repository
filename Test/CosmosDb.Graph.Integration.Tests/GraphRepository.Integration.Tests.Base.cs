using System;
using System.Linq;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using CosmosDb.Graph;
using CosmosDb.Graph.Interfaces;
using CosmosDb.Graph.TestStubs;

namespace CosmosDb.Graph.Integration.Tests
{
    public class GraphRepositoryIntegrationTestsBase : IDisposable
    {
        private readonly string _databaseIdentifier;
        private readonly string _graphIdentifier;
        protected readonly GraphRepositoryStub _sut;

        public GraphRepositoryIntegrationTestsBase()
        {
            _databaseIdentifier = Guid.NewGuid().ToString();
            _graphIdentifier = Guid.NewGuid().ToString();

            _sut = new GraphRepositoryStub(
                CosmosDbConnection.CreateCosmosDbConnection(
                "https://localhost:8081",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                _databaseIdentifier,
                _graphIdentifier,
                throughput: 400,
                connectionMode: ConnectionMode.Gateway,
                protocol: Protocol.Tcp),
                new GremlinQueryProvider(),
                new VertexConverter(),
                new EdgeConverter()
            );
        }

        protected void ClearGraph()
        {
            _sut.ExecuteScalarGremlinQuery("g.V().drop()").Wait();
        }

        protected int CountVertices()
        {
            var response = _sut.ExecuteScalarGremlinQuery("g.V().count()").Result;
            var responseArray = JArray.Parse(response);
            var element = responseArray.FirstOrDefault();

            return element != null 
                ? element.Value<int>() 
                : throw new NullReferenceException(nameof(CountVertices));
        }

        protected int CountEdges()
        {
            var response = _sut.ExecuteScalarGremlinQuery("g.E().count()").Result;
            var responseArray = JArray.Parse(response);
            var element = responseArray.FirstOrDefault();

            return element != null 
                ? element.Value<int>() 
                : throw new NullReferenceException(nameof(CountEdges));
        }

        public void Dispose()
        {
            if (_sut.Client != null)
            {
                _sut.Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseIdentifier)).Wait();         
                _sut.Dispose();
            }
        }
    }
}