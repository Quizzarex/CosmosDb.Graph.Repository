using System.Threading.Tasks;
using System.Collections.Generic;

using CosmosDb.Graph;
using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph.TestStubs
{
    public class GraphRepositoryStub : GraphRepository
    {
        public GraphRepositoryStub(
            ICosmosDbConnection cosmosDbConnection, 
            IGremlinQueryProvider gremlinQueryProvider, 
            IVertexConverter vertexConverter, 
            IEdgeConverter edgeConverter)
            : base(cosmosDbConnection, gremlinQueryProvider, vertexConverter, edgeConverter) {}

        public new async Task<string> ExecuteScalarGremlinQuery(string gremlinQuery) 
            => await base.ExecuteScalarGremlinQuery(gremlinQuery);

        public new async Task<IEnumerable<T>> ExecuteGremlinQuery<T>(string gremlinQuery) 
            => await base.ExecuteGremlinQuery<T>(gremlinQuery);

        public new async Task AddVertex<T>(T vertex) where T : VertexBase
            => await base.AddVertex<T>(vertex);

        public new async Task AddEdge<T>(T edge) where T : EdgeBase
            => await base.AddEdge<T>(edge);
        
        public new async Task<T> GetVertex<T>(string id) where T : VertexBase, new()
            => await base.GetVertex<T>(id);
        
        public new async Task<IEnumerable<T>> GetAllVertices<T>() where T : VertexBase, new()
            => await base.GetAllVertices<T>();

        public new async Task<T> GetEdge<T>(string id) where T : EdgeBase, new()
            => await base.GetEdge<T>(id);
        
        public new async Task<T> GetEdge<T>(string sourceId, string targetId) where T : EdgeBase, new()
            => await base.GetEdge<T>(sourceId, targetId);

        public new async Task<IEnumerable<T>> GetEdgesWithSourceId<T>(string sourceId) where T : EdgeBase, new()
            => await base.GetEdgesWithSourceId<T>(sourceId);

        public new async Task<IEnumerable<T>> GetEdgesWithTargetId<T>(string targetId) where T : EdgeBase, new()
            => await base.GetEdgesWithTargetId<T>(targetId);
        
        public new async Task<IEnumerable<T>> GetAllEdges<T>() where T : EdgeBase, new()
            => await base.GetAllEdges<T>();

        public new async Task UpdateVertex<T>(T vertex) where T : VertexBase
            => await base.UpdateVertex<T>(vertex);
        
        public new async Task UpdateEdge<T>(T edge) where T : EdgeBase
            => await base.UpdateEdge<T>(edge);
        
        public new async Task RemoveVertex(string id)
            => await base.RemoveVertex(id);
        
        public new async Task RemoveEdge<T>(string sourceId, string targetId) where T : EdgeBase
            => await base.RemoveEdge<T>(sourceId, targetId);

        public new async Task RemoveEdge<T>(T edge) where T : EdgeBase
            => await base.RemoveEdge<T>(edge);
        
        public new async Task RemoveEdge(string id)
            => await base.RemoveEdge(id);

        public new async Task<IEnumerable<TVertex>> ForwardTraverseVertices<TVertex, TEdge>(string id, int level = 1) 
            where TVertex : VertexBase, new()
            where TEdge : EdgeBase
            => await base.ForwardTraverseVertices<TVertex, TEdge>(id, level);

        public new async Task<IEnumerable<TVertex>> ReverseTraverseVertices<TVertex, TEdge>(string id, int level = 1) 
            where TVertex : VertexBase, new()
            where TEdge : EdgeBase
            => await base.ReverseTraverseVertices<TVertex, TEdge>(id, level);
    }
}