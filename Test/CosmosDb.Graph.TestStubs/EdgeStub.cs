using System;
using CosmosDb.Graph.Interfaces;

namespace CosmosDb.Graph.TestStubs
{
    public class EdgeStub : EdgeBase
    {
        public bool Bool { get; set; }
        public byte Byte { get; set; }
        public char Char { get; set; }
        public int Integer { get; set; }
        public double Double { get; set; }
        public string String { get; set; }
        public DateTime TimeStamp { get; set; }

        public static EdgeStub CreateEdgeStub(string sourceId, string targetId, string id = null)
        {
            return new EdgeStub
            {
                id = id != null ? id : Guid.NewGuid().ToString(),
                SourceId = sourceId,
                TargetId = targetId,
                Bool = true,
                Byte = 255,
                Char = 'A',
                Integer = 2,
                Double = -5.678,
                String = "Hello Vertex",
                TimeStamp = new DateTime(2017, 09, 19, 10, 45, 52, DateTimeKind.Utc),
            };
        }

        public override bool Equals(object obj)  
        {
            var item = obj as EdgeStub;
            
            if (item == null)
                return false;

            return  id == item.id &&
                    Bool == item.Bool &&
                    Byte == item.Byte &&
                    Char == item.Char &&
                    Integer == item.Integer &&
                    Double == item.Double &&
                    String == item.String &&
                    TimeStamp == item.TimeStamp;
        }

        public override int GetHashCode()
            => id.GetHashCode();
    }
}