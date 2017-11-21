using Xunit;
using System;

using CosmosDb.Graph.TestStubs;

namespace CosmosDb.Graph.Tests
{
    public class VertexConverterTests
    {
        [Fact]
        public void ToObject__PassingNull__ShouldThrowArgumentNullException()
        {
            var sut = new VertexConverter();

            Assert.Throws<ArgumentNullException>(() => sut.ToObject<VertexStub>(null));
        }

        [Fact]
        public void ToObject__PassingVertex__ShouldReturnObject()
        {
            /// As "Microsoft.Azure.Graphs.Elements.Vertex" is sealed and has an internal constructor, 
            /// a way of testing this method will have to be deviced as "ToObject" has a hard dependency on "Vertex".
        }
    }
}