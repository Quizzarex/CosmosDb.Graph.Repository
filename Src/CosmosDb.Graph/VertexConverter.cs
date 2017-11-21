using System;
using System.Reflection;
using Microsoft.Azure.Graphs.Elements;

using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph
{
    public class VertexConverter : IVertexConverter
    {
        public T ToObject<T>(Vertex vertex) where T : VertexBase, new()
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            var objectType = typeof(T);
            var obj = new T { id = vertex.Id.ToString() };

            foreach (var vertexProperty in vertex.GetVertexProperties())
            {
                var property = objectType.GetProperty(vertexProperty.Key);

                property?.SetValue(obj, Convert.ChangeType(vertexProperty.Value, property.PropertyType));
            }

            return obj;
        }
    }
}