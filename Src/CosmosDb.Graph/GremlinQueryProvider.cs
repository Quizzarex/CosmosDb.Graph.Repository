using System;
using System.Text;
using System.Linq;
using System.Reflection;

using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph
{
    public class GremlinQueryProvider : IGremlinQueryProvider
    {
        public string AddVertexQuery<T>(T obj) where T : VertexBase
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var objectType = typeof(T);
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($"g.addV('{objectType.Name}')");

            foreach (var property in objectType.GetProperties())
            {
                queryBuilder.Append($".property('{property.Name}', '{property.GetValue(obj)}')");
            }

            return queryBuilder.ToString();
        }
        
        public string RemoveVertexQuery(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return $"g.V('{id}').drop()";
        }

        public string GetVertexQuery<T>(string id) where T : VertexBase
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        
            return $"g.V('{id}').hasLabel('{typeof(T).Name}')";
        }

        public string GetVerticesQuery<T>() where T : VertexBase 
            => $"g.V().hasLabel('{typeof(T).Name}')";

        public string UpdateVertexQuery<T>(T obj) where T : VertexBase
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var objectType = typeof(T);
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($"g.V('{obj.id}')");

            foreach (var property in objectType.GetProperties())
            {
                if (property.Name != "id")
                    queryBuilder.Append($".property('{property.Name}', '{property.GetValue(obj)}')");
            }

            return queryBuilder.ToString();
        }

        public string ForwardTraverseVerticesQuery<TVertex, TEdge>(string id, int level = 1)
            where TVertex : VertexBase, new()
            where TEdge : EdgeBase
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var objectType = typeof(TVertex);
            var edgeType = typeof(TEdge);
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($"g.V('{id}')");

            for (var i = 0; i < level; i++)
            {
                queryBuilder.Append($".outE('{edgeType.Name}').inV().hasLabel('{objectType.Name}')");
            }

            return queryBuilder.ToString();
        }

        public string ReverseTraverseVerticesQuery<TVertex, TEdge>(string id, int level = 1)
            where TVertex : VertexBase, new()
            where TEdge : EdgeBase
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var objectType = typeof(TVertex);
            var edgeType = typeof(TEdge);
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($"g.V('{id}')");

            for (var i = 0; i < level; i++)
            {
                queryBuilder.Append($".inE('{edgeType.Name}').outV().hasLabel('{objectType.Name}')");
            }

            return queryBuilder.ToString();
        }

        public string AddEdgeQuery<T>(T edge) where T : EdgeBase
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            var edgeType = typeof(T);
            var edgeProperties = edgeType.GetProperties();
            var knownProperties = new[] { "SourceId", "TargetId" };
            var notKnownEdgePropertities = edgeProperties.Where(edgeProperty => !knownProperties.Any(knownProperty => edgeProperty.Name == knownProperty));
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($"g.V('{edge.SourceId}').addE('{typeof(T).Name}')");

            foreach (var property in notKnownEdgePropertities)
            {
                queryBuilder.Append($".property('{property.Name}', '{property.GetValue(edge)}')");
            }

            queryBuilder.Append($".to(g.V('{edge.TargetId}'))");

            return queryBuilder.ToString();
        }
        
        public string RemoveEdgeQuery<T>(string sourceId, string targetId) where T : EdgeBase
        {
            if (string.IsNullOrEmpty(sourceId)) throw new ArgumentNullException(nameof(sourceId));
            if (string.IsNullOrEmpty(targetId)) throw new ArgumentNullException(nameof(targetId));

            return $"g.V('{sourceId}').outE('{typeof(T).Name}').where(inV().has('id', '{targetId}')).drop()";
        }

        public string RemoveEdgeQuery<T>(T edge) where T : EdgeBase
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));
            
            return RemoveEdgeQuery<T>(edge.SourceId, edge.TargetId);
        }

        public string RemoveEdgeQuery(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return $"g.E('{id}').drop()";
        }
        
        public string GetEdgeQuery(string id) 
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return $"g.E('{id}')";
        }

        public string GetEdgeQuery<T>(string sourceId, string targetId) where T : EdgeBase
        {
            if (string.IsNullOrEmpty(sourceId)) throw new ArgumentNullException(nameof(sourceId));
            if (string.IsNullOrEmpty(targetId)) throw new ArgumentNullException(nameof(targetId));

            return $"g.V('{sourceId}').outE('{typeof(T).Name}').where(inV().has('id', '{targetId}'))";
        }

        public string GetEdgeQuery<T>(T edge) where T : EdgeBase
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            return GetEdgeQuery<T>(edge.SourceId, edge.TargetId);
        }

        public string GetEdgesWithSourceIdQuery<T>(string sourceId) where T : EdgeBase
        {
            if (string.IsNullOrEmpty(sourceId)) throw new ArgumentNullException(nameof(sourceId));

            return $"g.E().hasLabel('{typeof(T).Name}').where(outV().has('id', '{sourceId}'))";
        }

        public string GetEdgesWithTargetIdQuery<T>(string targetId) where T : EdgeBase
        {
            if (string.IsNullOrEmpty(targetId)) throw new ArgumentNullException(nameof(targetId));

            return $"g.E().hasLabel('{typeof(T).Name}').where(inV().has('id', '{targetId}'))";
        }

        public string GetEdgesQuery<T>() where T : EdgeBase 
            => $"g.E().hasLabel('{typeof(T).Name}')";

        public string UpdateEdgeQuery<T>(T edge) where T : EdgeBase
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            var edgeType = typeof(T);
            var edgeProperties = edgeType.GetProperties();
            var knownProperties = new[] { "id", "SourceId", "TargetId" };
            var notKnownEdgePropertities = edgeProperties.Where(edgeProperty => !knownProperties.Any(knownProperty => edgeProperty.Name == knownProperty));
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($"g.E('{edge.id}')");

            foreach (var property in notKnownEdgePropertities)
            {
                queryBuilder.Append($".property('{property.Name}', '{property.GetValue(edge)}')");
            }

            return queryBuilder.ToString();
        }
    }
}