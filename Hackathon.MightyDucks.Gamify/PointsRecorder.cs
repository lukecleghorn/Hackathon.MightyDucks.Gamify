using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Hackathon.MightyDucks.Gamify
{
    public static class PointsRecorder
    {
        private const string TableName = "PointsRecorder";
        private const string RouteAddPoints = "points/add";
        private const string RouteGetPoints = "points/get";
        private const string RouteReset = "reset";
        private const string PartitionKey = "MightyDucks";

        [FunctionName("PointsRecorder")]
        [return: Table(TableName)]
        public static async Task<PointsReceived> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteAddPoints)]HttpRequestMessage req, 
            [Table(TableName)]CloudTable table,
            TraceWriter log)
        {
            // parse query parameter
            var id = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
                .Value;
            var pointsQuery = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "points", true) == 0)
                .Value;


            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            id = id ?? data?.id;
            pointsQuery = pointsQuery ?? data?.pointsQuery;
            var points = long.Parse(pointsQuery);

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

        [FunctionName("GetPoints")]
        public static async Task<HttpResponseMessage> TableInput(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteGetPoints)]HttpRequestMessage req, 
            [Table(TableName, PartitionKey)] IQueryable<PointsReceived> pointsReceived,
            [Table(TableName)] CloudTable table,
            TraceWriter log)
        {
            var id = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
                .Value;
            dynamic data = await req.Content.ReadAsAsync<object>();

            id = id ?? data?.id;

            var pointsRow = pointsReceived.Where(p => p.RowKey == id).ToList().FirstOrDefault();

            if (pointsRow == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var points = JsonConvert.SerializeObject(new {pointsRow.Points});

            return new HttpResponseMessage
            {
                Content = new StringContent(points, Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.OK
            };
        }

        [FunctionName("ResetTable")]
        public static async Task<HttpResponseMessage> ResetTable(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = RouteReset)]HttpRequestMessage req,
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

            public long Points { get; set; }
        }
    }
}
