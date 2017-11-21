using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosDb.Graph
{
    public class CosmosDbConnection : IDisposable, ICosmosDbConnection
    {
        public DocumentClient Client { get; private set; }
        public DocumentCollection Collection { get; private set; }
        
        public static CosmosDbConnection CreateCosmosDbConnection(string endpoint, string authorizationKey, string database, string collection, int throughput = 400, ConnectionMode connectionMode = ConnectionMode.Direct, Protocol protocol = Protocol.Tcp)
        {
            var connection = new CosmosDbConnection
            {
                Client = new DocumentClient(new Uri(endpoint), authorizationKey, new ConnectionPolicy{
                    ConnectionMode = connectionMode,
                    ConnectionProtocol = protocol
                })
            };

            connection.Client.CreateDatabaseIfNotExistsAsync(new Database {Id = database}).Wait();

            connection.Collection = connection.Client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(database),
                new DocumentCollection {Id = collection}, new RequestOptions()
                {
                    OfferThroughput = throughput
                }).Result;
            
            return connection;
        }

        public void Dispose()
        {
            if (Client != null)
                Client.Dispose();
        }
    }
}