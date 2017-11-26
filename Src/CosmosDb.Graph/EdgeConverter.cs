using System;
using System.Reflection;
using Microsoft.Azure.Graphs.Elements;

using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph
{
    public class EdgeConverter : IEdgeConverter
    {
        public T ToObject<T>(Edge edge) where T : EdgeBase, new()
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            var objectType = typeof(T);
            var obj = new T 
            { 
                id = edge.Id.ToString(),
                SourceId = edge.OutVertexId.ToString(),
                TargetId = edge.InVertexId.ToString()
            };

            foreach (var edgeProperty in edge.GetProperties())
            {
                var property = objectType.GetProperty(edgeProperty.Key);

                property?.SetValue(obj, Convert.ChangeType(edgeProperty.Value, property.PropertyType));
            }

            return obj;
        }
    }
}