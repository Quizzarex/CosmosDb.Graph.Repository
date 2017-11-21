using Xunit;
using System;

using CosmosDb.Graph.TestStubs;

namespace CosmosDb.Graph.Tests
{
    public class EdgeConverterTests
    {
        [Fact]
        public void ToObject__PassingNull__ShouldThrowArgumentNullException()
        {
            var sut = new EdgeConverter();

            Assert.Throws<ArgumentNullException>(() => sut.ToObject<EdgeStub>(null));
        }

        [Fact]
        public void ToObject__PassingEdge__ShouldReturnObject()
        {
        }
    }
}