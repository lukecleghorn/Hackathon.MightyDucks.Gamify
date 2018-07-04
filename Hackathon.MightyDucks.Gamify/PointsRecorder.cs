using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace Hackathon.MightyDucks.Gamify
{
    public static class PointsRecorder
    {
        private const string TableName = "PointsRecorder";
        private const string Route = "points/send/{id?}/{points?}";

        [FunctionName("PointsRecorder")]
        [return: Table(TableName)]
        public static async Task<PointsReceived> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)]HttpRequestMessage req, 
            [Table(TableName)]CloudTable table,
            TraceWriter log)
        {
            List<KeyValuePair<string, string>> values = req.GetQueryNameValuePairs().ToList();

            return new PointsReceived { PartitionKey = "MightyDucks", RowKey = values.ElementAt(0).Value, Points = values.ElementAt(1).Value};
        }

        public class PointsReceived : TableEntity
        {
            public string Id
            {
                get => RowKey;
                set => RowKey = value;
            }

            public string Points { get; set; }
        }
    }
}
