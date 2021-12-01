NOTE: Development discontinued!

# CosmosDb.Graph.Repository
A light-weight .Net Standard/Framework library for CosmosDb graph databases.

![Build status](https://quizzarex.visualstudio.com/_apis/public/build/definitions/dd53a0f6-afa4-4279-b051-b592aef8424d/1/badge "Build Status")

## Overview
This light-weight library was written to make a strongly typed graph with respect to vertex and edge elements inside the graph. It allow developers to make simple POCO classes which is then serialized/deserialized by the library for convinient write/read operations to the graph. The library supports all the CRUD operations on vertices and edges, including traversal of vertices. In this alpha version of the library, lists, sets, enums and other complex types are not supported by the serialization/deserialization mechanics.

Requires either .NET Framework 4.6.1 or .NET Standard 1.6

## Installation
CosmosDb.Graph.Repository nuget can be [retrieved](https://www.nuget.org/packages/CosmosDb.Graph.Repository/1.0.0-alpha "Get CosmosDb.Graph.Repository @ nuget.org") from nuget.org

## Usage
The design of this library is very simple and serves to deliver flexibility for the developer. The main class' of this library is the abstract `GraphRepository` class which implements the basic repository CRUD operations and methods for traversal of vertices. The two other building blocks in this library are the `VertexBase` and the `EdgeBase` classes, which serves as base for either vertex or edge elements in the repository.

### Example
A use case of the library could be a User-Group graph repository. In this case two vertex types and one edge type are needed, vertex-type #1: `Group`, vertex-type #2: `User`, edge-type `UserFollowsGroup`. The following example shows a very simple implementation of such graph repository.

Here is an example of the `Group` vertex type:
```csharp
public class Group : VertexBase
{
    public string Name { get; set; }
    public string Purpose { get; set; }
}
```

An example of the `User` vertex type:
```csharp
public class User : VertexBase
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Birthday { get; set; }
}
```

Example of `UserFollowsGroup` edge implementation:
```csharp
public class UserFollowsGroup : EdgeBase
{
    public bool IsOwner { get; set; }
}
```

Repository implementation example:
```csharp
public class UserGroupUserRelations : GraphRepository
{
    public UserGroupRepository(ICosmosDbConnection cosmosDbConnection, IGremlinQueryProvider gremlinQueryProvider, IVertexConverter vertexConverter, IEdgeConverter edgeConverter) 
        : base(cosmosDbConnection, gremlinQueryProvider, vertexConverter, edgeConverter) {}
    public async void AddUser(User user) => await base.AddVertex(user);
    public async void AddGroup(Group group) => await base.AddVertex(group);
    public async void AddUserFollowsGroup(UserFollowsGroup userFollowsGroup) => await base.AddEdge(userFollowsGroup);
    public async Task<IEnumerable<User>> GetAllUsers() => await base.GetAllVertices<User>();
    public async Task<IEnumerable<Group>> GetAllGroups() => await base.GetAllVertices<Group>();
    public async Task<User> GetUserById(string id) => await base.GetVertex<User>(id);
    public async Task<Group> GetGroupById(string id) => await base.GetVertex<Group>(id);
    public async Task<IEnumerable<Group>> GetGroupsThatUserFollows(string userId) => await base.ForwardTraverseVertices<Group, UserFollowsGroup>(userId);
    public async Task<IEnumerable<User>> GetUsersThatFollowsGroup(string groupId) => await base.ReverseTraverseVertices<User, UserFollowsGroup>(groupId);

    public async Task<User> GetOwnerOfGroup(string groupId)
    {
        var groupHasFollowerEdges = await base.GetEdgesWithTargetId<UserFollowsGroup>(groupId);
        var ownersEdge = groupHasFollowerEdges.FirstOrDefault(edge => edge.IsOwner == true);

        return ownersEdge != null
            ? await base.GetVertex<User>(ownersEdge.SourceId)
            : null;
    }
}
```

Example of how to instantiate and use the custom repository:
A connection to the database is created with name of database and collection is provided together with the url and authentication key, this is passed on to custom repository instantiation.
```csharp
var dbConnection = CosmosDbConnection.CreateCosmosDbConnection(
    "https://localhost:8081",
    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "DatabaseName",
    "GraphName");

var graph = new UserGroupRepository(dbConnection, new GremlinQueryProvider(), new VertexConverter(), new EdgeConverter());
```

The connection created in the previous example is creating a connection to the local Cosmos DB emulator, the factory method assumes a trhoughput of 400 RU/s, connection mode is `Direct` and the protocol is set to be `Tcp`. The assumptions about mode and protocol is due to the performance tips mentioned [here @ microsoft.com](https://docs.microsoft.com/da-dk/azure/cosmos-db/performance-tips "Performance tips for Azure Cosmos DB and .NET @ Microsoft.com"). [This issue report](https://github.com/Azure/azure-documentdb-dotnet/issues/185 "Azure/azure-documentdb-dotnet @ github.com") @ github.com states that the connection mode `Direct` is bugged for local emulator and will cause a `ServiceUnavailableException`. The following example show how to work around this issue for local development and testing.

```csharp
var dbConnection = CosmosDbConnection.CreateCosmosDbConnection(
    "https://localhost:8081",
    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "DatabaseName",
    "GraphName",
    throughput: 400,
    connectionMode: ConnectionMode.Gateway,
    protocol: Protocol.Tcp);
```

## GraphRepository description
This section provides a brief description on class methods in the abstract `GraphRepository` class.

+ `protected GraphRepository(ICosmosDbConnection cosmosDbConnection, IGremlinQueryProvider gremlinQueryProvider, IVertexConverter vertexConverter, IEdgeConverter edgeConverter)`

   All the interfaces and their implementation are provided by the library and these are found in the namespace `CosmosDb.Graph`. In the above examples are shown how to instantiate the custom repository.

+ `protected async Task<string> ExecuteScalarGremlinQuery(string gremlinQuery)`

   This method allows call/send custom Gremlin queries from the custom repository and return JSON formatted string of the response.

+ `protected async Task<IEnumerable<T>> ExecuteGremlinQuery<T>(string gremlinQuery)`

   This method return a list of `T`with respect to some custom Gremlin query, either Vertex or Edge sittíng in the namespace `Microsoft.Azure.Graphs.Elements`.

+ `protected async Task AddVertex<T>(T vertex) where T : VertexBase`

   Adds vertex to graph of type `T`, notice that `T` has to inherit from `VertexBase`, this method has no return type.

+ `protected async Task AddEdge<T>(T edge) where T : EdgeBase`

   Adds edge to graph of type `T`, notice that `T` has to inherit from `EdgeBase`, this method has no return type.

+ `protected async Task<T> GetVertex<T>(string id) where T : VertexBase, new()`

   Return vertex of type `T` having specific `id`.

+ `protected async Task<IEnumerable<T>> GetAllVertices<T>() where T : VertexBase, new()`

   Return all vertices of type `T`.

+ `protected async Task<T> GetEdge<T>(string id) where T : EdgeBase, new()`

   Return edge of type `T` having specific `id`.

+ `protected async Task<T> GetEdge<T>(string sourceId, string targetId) where T : EdgeBase, new()`

   Return edge of type `T` having specific `sourceId` and `targetId`.

+ `protected async Task<IEnumerable<T>> GetEdgesWithSourceId<T>(string sourceId) where T : EdgeBase, new()`

   Return edges of type `T` having specific `sourceId`.

+ `protected async Task<IEnumerable<T>> GetEdgesWithTargetId<T>(string targetId) where T : EdgeBase, new()`

   Return edges of type `T` having specific `targetId`.

+ `protected async Task<IEnumerable<T>> GetAllEdges<T>() where T : EdgeBase, new()`

   Return all edges of type `T`.

+ `protected async Task UpdateVertex<T>(T vertex) where T : VertexBase`

   Update vertex of type `T`.

+ `protected async Task UpdateEdge<T>(T edge) where T : EdgeBase`

   Update edge of type `T`.

+ `protected async Task RemoveVertex(string id)`

   Remove vertex with `id`.

+ `protected async Task RemoveEdge(string id)`

   Remove edge with `id`.

+ `protected async Task RemoveEdge<T>(string sourceId, string targetId) where T : EdgeBase`

   Remove edge with `sourceId` and `targetId`.

+ `protected async Task RemoveEdge<T>(T edge) where T : EdgeBase`

   Remove edge of type `T`.

+ `protected async Task<IEnumerable<TVertex>> ForwardTraverseVertices<TVertex, TEdge>(string id, int level = 1)`

   Forward traversal (from source to target vertex) on vertices of type `TVertex` over edges of type `TEdge`, where `id` corresponds to the id of the root vertex of the traversal and level is the depth of traversal. Return leaf vertices of the traversal.

+ `protected async Task<IEnumerable<TVertex>> ReverseTraverseVertices<TVertex, TEdge>(string id, int level = 1)`

   Reverse traversal (from target to source vertex) on vertices of type `TVertex` over edges of type `TEdge`, where `id` corresponds to the id of the root vertex of the traversal and level is the depth of traversal. Return leaf vertices of the traversal.

## Future Support
+ Serialization/Deserialization mechanics should support complex objects such as lists, sets, enums, *etc.*
