using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosDb.Graph
{
    public interface ICosmosDbConnection
    {
        DocumentClient Client { get; }
        DocumentCollection Collection { get; }
    }
}