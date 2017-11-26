using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Graphs.Elements;
using Newtonsoft.Json;

using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph
{
    public abstract class GraphRepository : IDisposable
    {
        private readonly ICosmosDbConnection _cosmosDbConnection;
        private readonly IGremlinQueryProvider _gremlinQueryProvider;
        private readonly IVertexConverter _vertexConverter;
        private readonly IEdgeConverter _edgeConverter;

        public DocumentClient Client => _cosmosDbConnection.Client;
        public DocumentCollection Graph => _cosmosDbConnection.Collection;

        protected GraphRepository(
            ICosmosDbConnection cosmosDbConnection, 
            IGremlinQueryProvider gremlinQueryProvider, 
            IVertexConverter vertexConverter, 
            IEdgeConverter edgeConverter)
        {
            _cosmosDbConnection = cosmosDbConnection;
            _gremlinQueryProvider = gremlinQueryProvider;
            _vertexConverter = vertexConverter;
            _edgeConverter = edgeConverter;
        }

        #region Execute Gremlin query

        protected async Task<string> ExecuteScalarGremlinQuery(string gremlinQuery)
        {
            if (string.IsNullOrEmpty(gremlinQuery)) throw new ArgumentNullException(nameof(gremlinQuery));

            var result = new StringBuilder();
            var query = Client.CreateGremlinQuery(Graph, gremlinQuery);

            while (query.HasMoreResults)
            {
                var obj = await query.ExecuteNextAsync();
                result.Append(JsonConvert.SerializeObject(obj));
            }

            return result.ToString();
        }
        protected async Task<IEnumerable<T>> ExecuteGremlinQuery<T>(string gremlinQuery)
        {
            if (typeof(Vertex) != typeof(T) && typeof(Edge) != typeof(T)) 
                throw new ArgumentException($"Generic parameter of {nameof(ExecuteGremlinQuery)} should match either {typeof(Vertex)} or {typeof(Edge)}, but was {typeof(T)}");
            if (string.IsNullOrEmpty(gremlinQuery)) 
                throw new ArgumentNullException(nameof(gremlinQuery));

            var result = new List<T>();
            var query = Client.CreateGremlinQuery<T>(Graph, gremlinQuery);

            while (query.HasMoreResults)
            {
                result.AddRange(await query.ExecuteNextAsync<T>());
            }

            return result;
        }

        #endregion

        #region Add methods

        protected async Task AddVertex<T>(T vertex) where T : VertexBase
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            await ExecuteScalarGremlinQuery(_gremlinQueryProvider.AddVertexQuery(vertex));
        }

        protected async Task AddEdge<T>(T edge) where T : EdgeBase
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            await ExecuteScalarGremlinQuery(_gremlinQueryProvider.AddEdgeQuery(edge));
        }

        #endregion

        #region Get methods

        protected async Task<T> GetVertex<T>(string id) where T : VertexBase, new()
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var vertices = (await ExecuteGremlinQuery<Vertex>(_gremlinQueryProvider.GetVertexQuery<T>(id))).ToArray();

            return vertices.Any() 
                ? _vertexConverter.ToObject<T>(vertices.First())
                : null;
        }

        protected async Task<IEnumerable<T>> GetAllVertices<T>() where T : VertexBase, new()
        {
            var vertices = (await ExecuteGremlinQuery<Vertex>(_gremlinQueryProvider.GetVerticesQuery<T>())).ToArray();

            return vertices.Select(vertex => _vertexConverter.ToObject<T>(vertex));
        }

        protected async Task<T> GetEdge<T>(string id) where T : EdgeBase, new()
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var edges = (await ExecuteGremlinQuery<Edge>(_gremlinQueryProvider.GetEdgeQuery(id))).ToArray();

            return edges.Any()
                ? _edgeConverter.ToObject<T>(edges.First())
                : null;
        }

        protected async Task<T> GetEdge<T>(string sourceId, string targetId) where T : EdgeBase, new()
        {
            if (string.IsNullOrEmpty(sourceId)) throw new ArgumentNullException(nameof(sourceId));
            if (string.IsNullOrEmpty(targetId)) throw new ArgumentNullException(nameof(targetId));

            var edges = (await ExecuteGremlinQuery<Edge>(_gremlinQueryProvider.GetEdgeQuery<T>(sourceId, targetId))).ToArray();

            return edges.Any()
                ? _edgeConverter.ToObject<T>(edges.First())
                : null;
        }

        protected async Task<IEnumerable<T>> GetEdgesWithSourceId<T>(string sourceId) where T : EdgeBase, new()
        {
            if (string.IsNullOrEmpty(sourceId)) throw new ArgumentNullException(nameof(sourceId));

            var edges = (await ExecuteGremlinQuery<Edge>(_gremlinQueryProvider.GetEdgesWithSourceIdQuery<T>(sourceId))).ToArray();

            return edges.Select(edge => _edgeConverter.ToObject<T>(edge));
        }

        protected async Task<IEnumerable<T>> GetEdgesWithTargetId<T>(string targetId) where T : EdgeBase, new()
        {
            if (string.IsNullOrEmpty(targetId)) throw new ArgumentNullException(nameof(targetId));

            var edges = (await ExecuteGremlinQuery<Edge>(_gremlinQueryProvider.GetEdgesWithTargetIdQuery<T>(targetId))).ToArray();

            return edges.Select(edge => _edgeConverter.ToObject<T>(edge));
        }

        protected async Task<IEnumerable<T>> GetAllEdges<T>() where T : EdgeBase, new()
        {
            var edges = (await ExecuteGremlinQuery<Edge>(_gremlinQueryProvider.GetEdgesQuery<T>())).ToArray();

            return edges.Select(edge => _edgeConverter.ToObject<T>(edge));
        }

        #endregion

        #region Update methods

        protected async Task UpdateVertex<T>(T vertex) where T : VertexBase
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            await ExecuteScalarGremlinQuery(_gremlinQueryProvider.UpdateVertexQuery(vertex));
        }

        protected async Task UpdateEdge<T>(T edge) where T : EdgeBase
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            await ExecuteScalarGremlinQuery(_gremlinQueryProvider.UpdateEdgeQuery(edge));
        }

        #endregion

        #region Remove methods

        protected async Task RemoveVertex(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException();

            await ExecuteScalarGremlinQuery(_gremlinQueryProvider.RemoveVertexQuery(id));
        }

        protected async Task RemoveEdge<T>(string sourceId, string targetId) where T : EdgeBase
        {
            if (string.IsNullOrEmpty(sourceId)) throw new ArgumentNullException(nameof(sourceId));
            if (string.IsNullOrEmpty(targetId)) throw new ArgumentNullException(nameof(targetId));

            await ExecuteScalarGremlinQuery(_gremlinQueryProvider.RemoveEdgeQuery<T>(sourceId, targetId));
        }

        protected async Task RemoveEdge<T>(T edge) where T : EdgeBase
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            await ExecuteScalarGremlinQuery(_gremlinQueryProvider.RemoveEdgeQuery(edge));
        }

        protected async Task RemoveEdge(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            await ExecuteScalarGremlinQuery(_gremlinQueryProvider.RemoveEdgeQuery(id));
        }

        #endregion

        #region Traversal methods

        protected async Task<IEnumerable<TVertex>> ForwardTraverseVertices<TVertex, TEdge>(string id, int level = 1) 
            where TVertex : VertexBase, new()
            where TEdge : EdgeBase
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var vertices = await ExecuteGremlinQuery<Vertex>(_gremlinQueryProvider.ForwardTraverseVerticesQuery<TVertex, TEdge>(id, level));

            return vertices.Select(vertex => _vertexConverter.ToObject<TVertex>(vertex));
        }
        
        protected async Task<IEnumerable<TVertex>> ReverseTraverseVertices<TVertex, TEdge>(string id, int level = 1) 
            where TVertex : VertexBase, new()
            where TEdge : EdgeBase
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var vertices = await ExecuteGremlinQuery<Vertex>(_gremlinQueryProvider.ReverseTraverseVerticesQuery<TVertex, TEdge>(id, level));

            return vertices.Select(vertex => _vertexConverter.ToObject<TVertex>(vertex));
        }

        #endregion

        public void Dispose()
        {
            if (Client != null)
                Client.Dispose();
        }
    }
}
