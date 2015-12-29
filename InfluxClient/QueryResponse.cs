using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InfluxClient
{
    [DataContract]
    public class QueryResponse
    {
        [DataMember(Name = "results")]
        public List<QueryResults> Results
        { get; set; }
    }

    [DataContract]
    public class QueryResults
    {
        [DataMember(Name = "series")]
        public List<ResultsSeries> Series
        { get; set; }
    }

    [DataContract]
    public class ResultsSeries
    {
        public string Name
        { get; set; }

        public List<string> Columns
        { get; set; }

        public List<object> Values
        { get; set; }
    }
}
