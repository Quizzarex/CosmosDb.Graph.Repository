using Microsoft.Azure.Graphs.Elements;

using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph
{
    public interface IEdgeConverter
    {
        T ToObject<T>(Edge edge) where T : EdgeBase, new();
    }
}