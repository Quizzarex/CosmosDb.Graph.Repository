namespace CosmosDb.Graph.Interfaces
{
    public abstract class EdgeBase
    {
        public string id { get; set; }
        public string SourceId { get; set; }
        public string TargetId { get; set; }
    }
}