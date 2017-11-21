using Microsoft.Azure.Graphs.Elements;

using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph
{
    public interface IVertexConverter
    {
        T ToObject<T>(Vertex vertex) where T : VertexBase, new();
    }
}