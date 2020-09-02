using System;
using System.IO;
using System.Threading.Tasks;
using MashupAzureFunctionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;

namespace MashupAzureFunctionApp
{
    public static class AzureStorageFunction
    {

        [FunctionName("AzureStorageFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = false;

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<MashupResultModel>(requestBody);

            if (data != null)
            { 
                result = await UploadToAzure(requestBody, data, log);
            }

            return result 
                ? (ActionResult)new OkObjectResult("Request body was successfully uploaded to azure table.")
                : new BadRequestObjectResult("Upload of request body was unsuccessful");
        }

        private static async Task<bool> UploadToAzure(string requestBody, MashupResultModel data, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString",
                EnvironmentVariableTarget.Process);

            const string tableName = "MashupTable";
            var entity = new MashupEntityModel {PartitionKey = data.MbId, RowKey = data.Name, JsonModel = requestBody};

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var cloudTable = tableClient.GetTableReference(tableName);


            return await InsertToTable(entity, cloudTable, log);

        }

        private static async Task<bool> InsertToTable(ITableEntity data, CloudTable cloudTable, ILogger log)
        {
            try
            {
                var insertOperation = TableOperation.InsertOrMerge(data);
                await cloudTable.ExecuteAsync(insertOperation);
            }
            catch (Exception e)
            {
                log.LogError($"Something went wrong! {e.Message}. {e.InnerException}.");

                return false;
            }

            return true;
        }
    }
}
