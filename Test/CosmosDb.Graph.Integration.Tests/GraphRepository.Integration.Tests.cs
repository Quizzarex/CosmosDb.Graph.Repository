using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;

using CosmosDb.Graph;
using CosmosDb.Graph.TestStubs;

namespace CosmosDb.Graph.Integration.Tests
{
    public class GraphRepositoryIntegrationTests : GraphRepositoryIntegrationTestsBase
    {
        [Fact]
        public void ExecuteScalarGremlinQuery__QueringVerticesCountOnEmptyGraph__AssertVerticesCountEqualZero()
        {
            var count = CountVertices();

            Assert.Equal(0, count);
        }

        [Fact]
        public void AddVertex__AddingOneVertexToClearGraph__AssertVertexCountEqualOne()
        {
            ClearGraph();

            var vertex = VertexStub.CreateVertexStub();

            _sut.AddVertex(vertex).Wait();

            var count = CountVertices();

            Assert.Equal(1, count);
        }

        [Fact]
        public void AddEdge__AddingOneEdgeToClearGraph__AssertEdgeCountEqualOne()
        {
            ClearGraph();

            var vertexA = VertexStub.CreateVertexStub();
            var vertexB = VertexStub.CreateVertexStub();
            var edge = EdgeStub.CreateEdgeStub(vertexA.id, vertexB.id);

            _sut.AddVertex(vertexA).Wait();
            _sut.AddVertex(vertexB).Wait();
            _sut.AddEdge(edge).Wait();

            var count = CountEdges();

            Assert.Equal(1, count);
        }
        
        [Fact]
        public void GetVertex__PassingValidStringId__AssertVertexIsReturnedWithExpectedContent()
        {
            ClearGraph();

            var vertex = VertexStub.CreateVertexStub();

            _sut.AddVertex(vertex).Wait();

            var result = _sut.GetVertex<VertexStub>(vertex.id).Result;
            
            Assert.Equal(vertex, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void GetAllVertices__GettingAllVertices__AssertVerticesAreReturnedWithExpectedContent(int count)
        {
            ClearGraph();

            var vertices = new Dictionary<string, VertexStub>();

            for (int i = 0; i < count; i++)
            {
                var vertex = VertexStub.CreateVertexStub();

                vertices[vertex.id] = vertex;

                _sut.AddVertex(vertex).Wait();
            }

            var result = _sut.GetAllVertices<VertexStub>().Result;

            Assert.Equal(count, result.Count());
            
            foreach (var resultVertex in result)
            {
                Assert.Equal(vertices[resultVertex.id], resultVertex);
            }
        }

        [Fact]
        public void GetEdge__PassingValidStringEdgeId__AssertEdgeIsReturnedWithExpectedContent()
        {
            ClearGraph();

            var vertexA = VertexStub.CreateVertexStub();
            var vertexB = VertexStub.CreateVertexStub();
            var edge = EdgeStub.CreateEdgeStub(vertexA.id, vertexB.id);

            _sut.AddVertex(vertexA).Wait();
            _sut.AddVertex(vertexB).Wait();
            _sut.AddEdge(edge).Wait();

            var result = _sut.GetEdge<EdgeStub>(edge.id).Result;

            Assert.Equal(edge, result);
        }

        [Fact]
        public void GetEdge__PassingValidVerticesStringIds__AssertEdgeIsReturnedWithExpectedContent()
        {
            ClearGraph();

            var vertexA = VertexStub.CreateVertexStub();
            var vertexB = VertexStub.CreateVertexStub();
            var edge = EdgeStub.CreateEdgeStub(vertexA.id, vertexB.id);

            _sut.AddVertex(vertexA).Wait();
            _sut.AddVertex(vertexB).Wait();
            _sut.AddEdge(edge).Wait();

            var result = _sut.GetEdge<EdgeStub>(vertexA.id, vertexB.id).Result;

            Assert.Equal(edge, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void GetEdgesWithSourceId__GettingEdgesWithSourceId__AssertExpectedNumberOfEdgesAreReturned(int numberOfExpectedEdges)
        {
            ClearGraph();

            var rootVertex = VertexStub.CreateVertexStub();
            var expectedVertexIds = new List<string>();

            _sut.AddVertex(rootVertex).Wait();

            for (var i = 0; i < numberOfExpectedEdges; i++)
            {
                var vertex = VertexStub.CreateVertexStub();
                var edge = EdgeStub.CreateEdgeStub(rootVertex.id, vertex.id);

                expectedVertexIds.Add(vertex.id);

                _sut.AddVertex(vertex).Wait();
                _sut.AddEdge(edge).Wait();
            }

            var edges = _sut.GetEdgesWithSourceId<EdgeStub>(rootVertex.id).Result;

            Assert.Equal(expectedVertexIds.Count, edges.Count());

            foreach (var edge in edges)
            {
                Assert.True(expectedVertexIds.Any(id => id == edge.TargetId));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void GetEdgesWithTargetId__GettingEdgesWithTargetId__AssertExpectedNumberOfEdgesAreReturned(int numberOfExpectedEdges)
        {
            ClearGraph();

            var rootVertex = VertexStub.CreateVertexStub();
            var expectedVertexIds = new List<string>();

            _sut.AddVertex(rootVertex).Wait();

            for (var i = 0; i < numberOfExpectedEdges; i++)
            {
                var vertex = VertexStub.CreateVertexStub();
                var edge = EdgeStub.CreateEdgeStub(vertex.id, rootVertex.id);

                expectedVertexIds.Add(vertex.id);

                _sut.AddVertex(vertex).Wait();
                _sut.AddEdge(edge).Wait();
            }

            var edges = _sut.GetEdgesWithTargetId<EdgeStub>(rootVertex.id).Result;

            Assert.Equal(expectedVertexIds.Count, edges.Count());

            foreach (var edge in edges)
            {
                Assert.True(expectedVertexIds.Any(id => id == edge.SourceId));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void GetAllEdges__GettingAllEdges__AssertEdgesAreReturnedWithExpectedContent(int count)
        {
            ClearGraph();

            var edges = new Dictionary<string, EdgeStub>();
            var previousVertex = VertexStub.CreateVertexStub();

            _sut.AddVertex(previousVertex).Wait();

            for (int i = 0; i < count; i++)
            {
                var nextVertex = VertexStub.CreateVertexStub();
                var edge = EdgeStub.CreateEdgeStub(previousVertex.id, nextVertex.id);

                _sut.AddVertex(nextVertex).Wait();
                _sut.AddEdge(edge).Wait();

                previousVertex = nextVertex;

                edges[edge.id] = edge;
            }

            var result = _sut.GetAllEdges<EdgeStub>().Result;

            Assert.Equal(count, result.Count());

            foreach (var resultEdge in result)
            {
                Assert.Equal(edges[resultEdge.id], resultEdge);
            }
        }

        [Fact]
        public void UpdateVertex__UpdatingVertex__AssertVertexHasBeenUpdated()
        {
            ClearGraph();

            var vertexA = VertexStub.CreateVertexStub();

            _sut.AddVertex(vertexA).Wait();

            var vertexB = _sut.GetVertex<VertexStub>(vertexA.id).Result;

            vertexB.Bool = false;
            vertexB.Char = 'B';
            vertexB.String = "Updated vertex";

            _sut.UpdateVertex(vertexB).Wait();

            var vertexC = _sut.GetVertex<VertexStub>(vertexA.id).Result;

            Assert.NotEqual(vertexA, vertexC);
            Assert.Equal(vertexB, vertexC);
        }

        [Fact]
        public void UpdateEdge__UpdatingEdge__AssertEdgeHasBeenUpdated()
        {
            ClearGraph();

            var vertexA = VertexStub.CreateVertexStub();
            var vertexB = VertexStub.CreateVertexStub();
            var edgeA = EdgeStub.CreateEdgeStub(vertexA.id, vertexB.id);

            _sut.AddVertex(vertexA).Wait();
            _sut.AddVertex(vertexB).Wait();
            _sut.AddEdge(edgeA).Wait();

            var edgeB = _sut.GetEdge<EdgeStub>(edgeA.id).Result;

            edgeB.Bool = false;
            edgeB.Char = 'B';
            edgeB.String = "Updated edge";

            _sut.UpdateEdge(edgeB).Wait();

            var edgeC = _sut.GetEdge<EdgeStub>(edgeA.id).Result;

            Assert.NotEqual(edgeA, edgeC);
            Assert.Equal(edgeB, edgeC);
        }

        [Fact]
        public void RemoveVertex__AddingVertexToClearGraphAndRemovingItById__AssertVerticesCountEqualZero()
        {
            ClearGraph();

            var vertex = VertexStub.CreateVertexStub();

            _sut.AddVertex(vertex).Wait();

            var count = CountVertices();

            Assert.Equal(1, count);

            _sut.RemoveVertex(vertex.id).Wait();

            count = CountVertices();

            Assert.Equal(0, count);
        }

        [Fact]
        public void RemoveEdge__AddingOneEdgeToClearGraphAndRemovingItBySourceAndTargetId__AssertEdgeCountEqualZero()
        {
            ClearGraph();

            var vertexA = VertexStub.CreateVertexStub();
            var vertexB = VertexStub.CreateVertexStub();
            var edge = EdgeStub.CreateEdgeStub(vertexA.id, vertexB.id);

            _sut.AddVertex(vertexA).Wait();
            _sut.AddVertex(vertexB).Wait();
            _sut.AddEdge(edge).Wait();

            var count = CountEdges();

            Assert.Equal(1, count);

            _sut.RemoveEdge<EdgeStub>(vertexA.id, vertexB.id).Wait();
            
            count = CountEdges();

            Assert.Equal(0, count);
        }

        [Fact]
        public void RemoveEdge__AddingOneEdgeToClearGraphAndRemovingItByEdge__AssertEdgeCountEqualZero()
        {
            ClearGraph();

            var vertexA = VertexStub.CreateVertexStub();
            var vertexB = VertexStub.CreateVertexStub();
            var edge = EdgeStub.CreateEdgeStub(vertexA.id, vertexB.id);

            _sut.AddVertex(vertexA).Wait();
            _sut.AddVertex(vertexB).Wait();
            _sut.AddEdge(edge).Wait();

            var count = CountEdges();

            Assert.Equal(1, count);

            _sut.RemoveEdge(edge).Wait();
            
            count = CountEdges();

            Assert.Equal(0, count);
        }

        [Fact]
        public void RemoveEdge__AddingOneEdgeToClearGraphAndRemovingItById__AssertEdgeCountEqualZero()
        {
            ClearGraph();

            var vertexA = VertexStub.CreateVertexStub();
            var vertexB = VertexStub.CreateVertexStub();
            var edge = EdgeStub.CreateEdgeStub(vertexA.id, vertexB.id);

            _sut.AddVertex(vertexA).Wait();
            _sut.AddVertex(vertexB).Wait();
            _sut.AddEdge(edge).Wait();

            var count = CountEdges();

            Assert.Equal(1, count);

            _sut.RemoveEdge(edge.id).Wait();
            
            count = CountEdges();

            Assert.Equal(0, count);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public void ForwardTraverseVertices__ForwardTraversingVertices__AssertVerticesAreReturnedWithExpectedContent(int numberOfChildVertices, int numberOfLayers)
        {
            ClearGraph();

            var rootVertex = VertexStub.CreateVertexStub();
            var layers = new List<Dictionary<string, VertexStub>>
            {
                new Dictionary<string, VertexStub>
                {
                    {rootVertex.id, rootVertex}
                }
            };

            _sut.AddVertex(rootVertex).Wait();

            for (var i = 0; i < numberOfLayers; i++)
            {
                layers.Add(new Dictionary<string, VertexStub>());

                foreach (var sourceVertex in layers[i].Values)
                {
                    for (var j = 0; j < numberOfChildVertices; j++)
                    {
                        var targetVertex = VertexStub.CreateVertexStub();
                        var edge = EdgeStub.CreateEdgeStub(sourceVertex.id, targetVertex.id);

                        _sut.AddVertex(targetVertex).Wait();
                        _sut.AddEdge(edge).Wait();

                        layers[i + 1][targetVertex.id] = targetVertex;
                    }
                }
            }

            var resultVertices = _sut.ForwardTraverseVertices<VertexStub, EdgeStub>(rootVertex.id, numberOfLayers).Result.ToList();

            Assert.Equal(layers[numberOfLayers].Count, resultVertices.Count);

            foreach (var result in resultVertices)
            {
                var vertex = layers[numberOfLayers][result.id];

                Assert.Equal(vertex, result);
            }
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public void ReverseTraverseVertices__ReverseTraversingVertices__AssertVerticesAreReturnedWithExpectedContent(int numberOfChildVertices, int numberOfLayers)
        {
            ClearGraph();

            var rootVertex = VertexStub.CreateVertexStub();
            var layers = new List<Dictionary<string, VertexStub>>
            {
                new Dictionary<string, VertexStub>
                {
                    {rootVertex.id, rootVertex}
                }
            };

            _sut.AddVertex(rootVertex).Wait();

            for (var i = 0; i < numberOfLayers; i++)
            {
                layers.Add(new Dictionary<string, VertexStub>());

                foreach (var sourceVertex in layers[i].Values)
                {
                    for (var j = 0; j < numberOfChildVertices; j++)
                    {
                        var targetVertex = VertexStub.CreateVertexStub();
                        var edge = EdgeStub.CreateEdgeStub(targetVertex.id, sourceVertex.id);

                        _sut.AddVertex(targetVertex).Wait();
                        _sut.AddEdge(edge).Wait();

                        layers[i + 1][targetVertex.id] = targetVertex;
                    }
                }
            }

            var resultVertices = _sut.ReverseTraverseVertices<VertexStub, EdgeStub>(rootVertex.id, numberOfLayers).Result.ToList();

            Assert.Equal(layers[numberOfLayers].Count, resultVertices.Count);

            foreach (var result in resultVertices)
            {
                var vertex = layers[numberOfLayers][result.id];

                Assert.Equal(vertex, result);
            }
        }
    }
}
