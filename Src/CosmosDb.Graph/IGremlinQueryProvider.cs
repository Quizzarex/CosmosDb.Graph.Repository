
using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph
{
    public interface IGremlinQueryProvider
    {
        string AddVertexQuery<T>(T obj) where T : VertexBase;
        string RemoveVertexQuery(string id);
        string GetVertexQuery<T>(string id) where T : VertexBase;
        string GetVerticesQuery<T>() where T : VertexBase;
        string UpdateVertexQuery<T>(T obj) where T : VertexBase;
        string ForwardTraverseVerticesQuery<TVertex, TEdge>(string id, int level = 1) where TVertex : VertexBase, new() where TEdge : EdgeBase;
        string ReverseTraverseVerticesQuery<TVertex, TEdge>(string id, int level = 1) where TVertex : VertexBase, new() where TEdge : EdgeBase;
        string AddEdgeQuery<T>(T edge) where T : EdgeBase;
        string RemoveEdgeQuery<T>(string sourceId, string targetId) where T : EdgeBase;
        string RemoveEdgeQuery<T>(T edge) where T : EdgeBase;
        string RemoveEdgeQuery(string id);
        string GetEdgeQuery(string id);
        string GetEdgeQuery<T>(string sourceId, string targetId) where T : EdgeBase;
        string GetEdgeQuery<T>(T edge) where T : EdgeBase;
        string GetEdgesWithSourceIdQuery<T>(string sourceId) where T : EdgeBase;
        string GetEdgesWithTargetIdQuery<T>(string targetId) where T : EdgeBase;
        string GetEdgesQuery<T>() where T : EdgeBase;
        string UpdateEdgeQuery<T>(T edge) where T : EdgeBase;
    }
}