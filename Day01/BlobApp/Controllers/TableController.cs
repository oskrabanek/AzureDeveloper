using System;
using System.Net.Http;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlobApp.Controllers
{
    [ApiController]
    [Route("tables")]
    public class TableController : ControllerBase
    {
        private Options _options;
        private const string TableName = "mytable";

        // Define a strongly typed entity by implementing the ITableEntity interface.
        public class OfficeSupplyEntity : ITableEntity
        {
            public string Product { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
        }


        public TableController(HttpClient httpClient, Options options)
        {
            _options = options;
        }

        [Route("")]
        [HttpGet]
        public ActionResult<string> Get()
        {
            TableServiceClient serviceClient = new TableServiceClient(_options.StorageConnectionString);
            TableClient tableClient = serviceClient.GetTableClient(TableName);
            TableItem table = serviceClient.CreateTableIfNotExists(TableName);
            var tableEntity = new OfficeSupplyEntity
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = "",
                Product = "Notebook",
                Price = 3.00,
                Quantity = 50
            };
            tableClient.AddEntity(tableEntity);

            return Ok();
        }
    }
}
