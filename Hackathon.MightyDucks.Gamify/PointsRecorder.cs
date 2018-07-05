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
        private const string RoutePoints = "points/send/{id?}/{points?}";
        private const string RouteReset = "reset";
        private const string PartitionKey = "MightyDucks";

        [FunctionName("PointsRecorder")]
        [return: Table(TableName)]
        public static async Task<PointsReceived> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RoutePoints)]HttpRequestMessage req, 
            [Table(TableName)]CloudTable table,
            TraceWriter log)
        {
            List<KeyValuePair<string, string>> values = req.GetQueryNameValuePairs().ToList();
            var id = values.ElementAt(0).Value;
            var points = Int16.Parse(values.ElementAt(1).Value);

            var operation = TableOperation.Retrieve<PointsReceived>(PartitionKey, id);
            var result = await table.ExecuteAsync(operation);

            if (result.Result != null)
            {
                var pointsReceiver = (PointsReceived) result.Result;
                pointsReceiver.Points += points;
                return pointsReceiver;
            }

            return new PointsReceived
            {
                PartitionKey = PartitionKey,
                Id = id,
                Points = points
            };
        }

        [FunctionName("ResetTable")]
        public static async Task<HttpResponseMessage> ResetTable(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteReset)]HttpRequestMessage req,
            [Table(TableName)]CloudTable table,
            TraceWriter log
        )
        {
            await table.DeleteAsync();
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        } 

        public class PointsReceived : TableEntity
        {
            public string Id
            {
                get => RowKey;
                set => RowKey = value;
            }

            public int Points { get; set; }
        }
    }
}
