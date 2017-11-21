using Xunit;
using System;
using System.Linq;

using CosmosDb.Graph.TestStubs;

namespace CosmosDb.Graph.Tests
{
    public class GremlinQueryProviderTests
    {
        private readonly GremlinQueryProvider _sut = new GremlinQueryProvider();

        [Fact]
        public void AddVertexQuery__PasingNull__AssertThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.AddVertexQuery<VertexStub>(null));
        }

        [Fact]
        public void AddVertexQuery__PassingVertexStub__AssertGremlinQuery()
        {
            var vertexStub = VertexStub.CreateVertexStub();

            var query = _sut.AddVertexQuery(vertexStub);

            Assert.StartsWith($"g.addV('{vertexStub.GetType().Name}')", query);

            foreach (var propertyInfo in vertexStub.GetType().GetProperties())
            {
                Assert.Contains($".property('{propertyInfo.Name}', '{propertyInfo.GetValue(vertexStub)}')", query);
            }
        }

        [Fact]
        public void RemoveVertexQuery__PassingNull__AssertThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.RemoveVertexQuery(null));
        }

        [Fact]
        public void RemoveVertexQuery__PassingEmptyString__AssertThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.RemoveVertexQuery(string.Empty));
        }

        [Fact]
        public void RemoveVertexQuery__PassingVertexId__AssertGremlinQuery()
        {
            var id = Guid.NewGuid().ToString();

            Assert.Equal($"g.V('{id}').drop()", _sut.RemoveVertexQuery(id));
        }

        [Fact]
        public void GetVertexQuery__PassingNull__AssertThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetVertexQuery<VertexStub>(null));
        }

        [Fact]
        public void GetVertexQuery__PassingEmptyString__AssertThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetVertexQuery<VertexStub>(string.Empty));
        }

        [Fact]
        public void GetVertexQuery__PassingId__AssertGremlinQuery()
        {
            var id = Guid.NewGuid().ToString();

            Assert.Equal($"g.V('{id}').hasLabel('{typeof(VertexStub).Name}')", _sut.GetVertexQuery<VertexStub>(id));
        }

        [Fact]
        public void GetVerticesQuery__AssertGremlinQuery()
        {
            Assert.Equal($"g.V().hasLabel('{typeof(VertexStub).Name}')", _sut.GetVerticesQuery<VertexStub>());
        }

        [Fact]
        public void UpdateVertexQuery__PassingNull__AssertGremlinQuery()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.UpdateVertexQuery<VertexStub>(null));
        }

        [Fact]
        public void UpdateVertexQuery__PassingVertexStub__AssertGremlinQuery()
        {
            var vertexStub = VertexStub.CreateVertexStub();

            var query = _sut.UpdateVertexQuery(vertexStub);

            Assert.StartsWith($"g.V('{vertexStub.id}')", query);

            foreach (var propertyInfo in vertexStub.GetType().GetProperties().Where(property => property.Name != "id"))
            {
                Assert.Contains($".property('{propertyInfo.Name}', '{propertyInfo.GetValue(vertexStub)}')", query);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TraverseVerticesQuery__PassingNullOrEmptyStringId__AssertThrowArgumentNullException(string id)
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ForwardTraverseVerticesQuery<VertexStub, EdgeStub>(id));
        }

        [Theory]
        [InlineData()]
        [InlineData(2)]
        [InlineData(7)]
        public void TraverseVerticesQuery__PassingId__AssertGremlinQuery(int level = 1)
        {
            var id = Guid.NewGuid().ToString();

            var query = _sut.ForwardTraverseVerticesQuery<VertexStub, EdgeStub>(id, level);

            Assert.StartsWith($"g.V('{id}')", query);

            var querySplit = query.Split(new [] { ".outE" }, StringSplitOptions.None);

            for (int i = 1; i <= level; i++)
            {
                Assert.Equal($"('{typeof(EdgeStub).Name}').inV().hasLabel('{typeof(VertexStub).Name}')", querySplit[i]);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ReverseTraverseVerticesQuery__PassingNullOrEmptyStringId__AssertThrowArgumentNullException(string id)
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ReverseTraverseVerticesQuery<VertexStub, EdgeStub>(id));
        }

        [Theory]
        [InlineData()]
        [InlineData(2)]
        [InlineData(7)]
        public void ReverseTraverseVerticesQuery__PassingId__AssertGremlinQuery(int level = 1)
        {
            var id = Guid.NewGuid().ToString();

            var query = _sut.ReverseTraverseVerticesQuery<VertexStub, EdgeStub>(id, level);

            Assert.StartsWith($"g.V('{id}')", query);

            var querySplit = query.Split(new [] { ".inE" }, StringSplitOptions.None);

            for (int i = 1; i <= level; i++)
            {
                Assert.Equal($"('{typeof(EdgeStub).Name}').outV().hasLabel('{typeof(VertexStub).Name}')", querySplit[i]);
            }
        }

        [Fact]
        public void AddEdgeQuery__PassingNull__AssertThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.AddEdgeQuery<EdgeStub>(null));
        }

        [Fact]
        public void AddEdgeQuery__PassingEdge__AssertGremlinQuery()
        {
            var edge = EdgeStub.CreateEdgeStub(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var query = _sut.AddEdgeQuery(edge);

            Assert.StartsWith($"g.V('{edge.SourceId}').addE('{edge.GetType().Name}')", query);
            Assert.EndsWith($".to(g.V('{edge.TargetId}'))", query);

            var edgeProperties = edge.GetType().GetProperties().Where(property => !(new [] { "SourceId", "TargetId" }).Any(p => property.Name == p));

            foreach (var property in edgeProperties)
            {
                Assert.Contains($".property('{property.Name}', '{property.GetValue(edge)}')", query);
            }
        }

        [Theory]
        [InlineData("a", null)]
        [InlineData(null, "b")]
        [InlineData("a", "")]
        [InlineData("", "b")]
        public void RemoveEdgeQuery__PassingNullOrEmptyStringIdsOrInvalidEdge__AssertThrowArgumentNullException(string sourceId, string targetId)
        {
            var edge = EdgeStub.CreateEdgeStub(sourceId, targetId);

            Assert.Throws<ArgumentNullException>(() => _sut.RemoveEdgeQuery(edge));
            Assert.Throws<ArgumentNullException>(() => _sut.RemoveEdgeQuery<EdgeStub>(sourceId, targetId));
        }

        [Fact]
        public void RemoveEdgeQuery__PassingNullEdge__AssertThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.RemoveEdgeQuery<EdgeStub>(null));
        }

        [Fact]
        public void RemoveEdgeQuery__PassingSourceIdAndTargetId__AssertGremlinQuery()
        {
            var sourceId = Guid.NewGuid().ToString();
            var targetId = Guid.NewGuid().ToString();
            
            var query = _sut.RemoveEdgeQuery<EdgeStub>(sourceId, targetId);

            Assert.Equal($"g.V('{sourceId}').outE('{typeof(EdgeStub).Name}').where(inV().has('id', '{targetId}')).drop()", query);
        }

        [Fact]
        public void RemoveEdgeQuery__PassingEdge__AssertGremlinQuery()
        {
            var sourceId = Guid.NewGuid().ToString();
            var targetId = Guid.NewGuid().ToString();
            var edge = EdgeStub.CreateEdgeStub(sourceId, targetId);

            var query = _sut.RemoveEdgeQuery(edge);

            Assert.Equal($"g.V('{sourceId}').outE('{edge.GetType().Name}').where(inV().has('id', '{targetId}')).drop()", query);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RemoveEdgeQuery__PassingNullOrEmptyEdgeId__AssertThrowArgumentNullException(string id)
        {
            Assert.Throws<ArgumentNullException>(() => _sut.RemoveEdgeQuery(id));
        }

        [Fact]
        public void RemoveEdgeQuery__PassingEdgeId__AssertGremlinQuery()
        {
            var id = Guid.NewGuid().ToString();

            var query = _sut.RemoveEdgeQuery(id);

            Assert.Equal($"g.E('{id}').drop()", query);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetEdgeQuery__PassingNullOrEmptyEdgeId__AssertThrowArgumentNullException(string id)
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetEdgeQuery(id));
        }

        [Fact]
        public void GetEdgeQuery__PassingEdgeId__AssertGremlinQuery()
        {
            var id = Guid.NewGuid().ToString();

            var query = _sut.GetEdgeQuery(id);

            Assert.Equal($"g.E('{id}')", query);
        }

        [Theory]
        [InlineData("a", null)]
        [InlineData(null, "b")]
        [InlineData("a", "")]
        [InlineData("", "b")]
        public void GetEdgeQuery__PassingNullOrEmptyStringIdsOrInvalidEdge__AssertThrowArgumentNullException(string sourceId, string targetId)
        {
            var edge = EdgeStub.CreateEdgeStub(sourceId, targetId);

            Assert.Throws<ArgumentNullException>(() => _sut.GetEdgeQuery(edge));
            Assert.Throws<ArgumentNullException>(() => _sut.GetEdgeQuery<EdgeStub>(sourceId, targetId));
        }

        [Fact]
        public void GetEdgeQuery__PassingNullEdge__AssertThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetEdgeQuery<EdgeStub>(null));
        }

        [Fact]
        public void GetEdgeQuery__PassingSourceIdAndTargetId__AssertGremlinQuery()
        {
            var sourceId = Guid.NewGuid().ToString();
            var targetId = Guid.NewGuid().ToString();
            
            var query = _sut.GetEdgeQuery<EdgeStub>(sourceId, targetId);

            Assert.Equal($"g.V('{sourceId}').outE('{typeof(EdgeStub).Name}').where(inV().has('id', '{targetId}'))", query);
        }

        [Fact]
        public void GetEdgeQuery__PassingEdge__AssertGremlinQuery()
        {
            var sourceId = Guid.NewGuid().ToString();
            var targetId = Guid.NewGuid().ToString();
            var edge = EdgeStub.CreateEdgeStub(sourceId, targetId);

            var query = _sut.GetEdgeQuery(edge);

            Assert.Equal($"g.V('{sourceId}').outE('{edge.GetType().Name}').where(inV().has('id', '{targetId}'))", query);
        }

        [Fact]
        public void GetEdgesQuery__NoParameters__AssertGremlinQuery()
        {
            var query = _sut.GetEdgesQuery<EdgeStub>();
            
            Assert.Equal($"g.E().hasLabel('{typeof(EdgeStub).Name}')", query);
        }
    }
}