
using Xunit;
using System;
using Microsoft.Azure.Graphs.Elements;

using CosmosDb.Graph.TestStubs;

namespace CosmosDb.Graph.Tests
{
    public class GraphRepositoryTests
    {
        private readonly GraphRepositoryStub _sut
            = new GraphRepositoryStub(
                new CosmosDbConnection(),
                new GremlinQueryProvider(),
                new VertexConverter(),
                new EdgeConverter());

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ExecuteScalarGremlinQuery__PassingNullOrEmptyGremlinQuery__AssertThrowArgumentNullException(string gremlinQuery) 
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ExecuteScalarGremlinQuery(gremlinQuery));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ExecuteGremlinQuery__PassingValidGenericParameterAndNullOrEmptyGremlinQuery__AssertThrowArgumentNullException(string gremlinQuery) 
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ExecuteGremlinQuery<Vertex>(gremlinQuery));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ExecuteGremlinQuery<Edge>(gremlinQuery));
        }

        [Fact]
        public void ExecuteGremlinQuery__PassingInvalidGenericParameter__AssertThrowArgumentException() 
            => Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ExecuteGremlinQuery<object>("g.V()"));

        [Fact]
        public void AddVertex__PassingNull__AssertThrowArgumentNullException()
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.AddVertex<VertexStub>(null));

        [Fact]
        public void AddEdge__PassingNull__AssertThrowArgumentNullException()
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.AddEdge<EdgeStub>(null));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetVertex__PassingNullOrEmptyStringId__AssertThrowArgumentNullException(string id)
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetVertex<VertexStub>(id));
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetEdge__PassingNullOrEmptyStringId__AssertThrowArgumentNullException(string id)
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetEdge<EdgeStub>(id));

        [Theory]
        [InlineData(null, "b")]
        [InlineData("a", null)]
        [InlineData("", "b")]
        [InlineData("a", "")]
        public void GetEdge__PassingNullAndEmptyStringIds__AssertThrowArgumentNullException(string sourceId, string targetId)
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetEdge<EdgeStub>(sourceId, targetId));

        [Fact]
        public void UpdateVertex__PassingNullVertex__AssertThrowArgumentNullException()
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.UpdateVertex<VertexStub>(null));

        [Fact]
        public void UpdateEdge__PassingNullEdge__AssertThrowArgumentNullException()
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.UpdateEdge<EdgeStub>(null));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RemoveVertex__PassingNullOrEmptyStringId__AssertThrowArgumentNullException(string id)
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.RemoveVertex(id));

        [Theory]
        [InlineData(null, "b")]
        [InlineData("a", null)]
        [InlineData("", "b")]
        [InlineData("a", "")]
        public void RemoveEdge__PassingNullAndEmptyStringIds__AssertThrowArgumentNullException(string sourceId, string targetId)
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.RemoveEdge<EdgeStub>(sourceId, targetId));

        [Fact]
        public void RemoveEdge__PassingNullEdge__AssertThrowArgumentNullException()
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.RemoveEdge<EdgeStub>(null));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RemoveEdge__PassingNullAndEmptyStringId__AssertThrowArgumentNullException(string id)
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.RemoveEdge(id));

        

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ForwardTraverseVertices__PassingNullOrEmptyStringId__AssertThrowArgumentNullException(string id)
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ForwardTraverseVertices<VertexStub, EdgeStub>(id));

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ReverseTraverseVertices__PassingNullOrEmptyStringId__AssertThrowArgumentNullException(string id)
            => Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ReverseTraverseVertices<VertexStub, EdgeStub>(id));
    }
}
