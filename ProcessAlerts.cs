using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using ResourceHealthAlertPOC.Models;
using ResourceHealthAlertPOC.Util;
using Microsoft.Azure.Cosmos;
using System.Configuration;

namespace ResourceHealthAlertPOC
{
    public static class ProcessAlerts
    {
        [FunctionName("ProcessAlerts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var resourceHealthAlert = JsonConvert.DeserializeObject<ResourceHealthAlert>(requestBody);
            
            var alertObj = CosmosHelper.GetDtoMapping(resourceHealthAlert);

            log.LogInformation("Resource Health Alert Id: " + alertObj.alertId);
            log.LogInformation("Resource Subscription: " + alertObj.subscriptionId);
            log.LogInformation("Resource Id: " + alertObj.resourceId);
            log.LogInformation("Resource Status: " + alertObj.currentHealthStatus);

            var collectionId = GetEnvironmentVariable("CosmosDb_Collection");
            var databaseId = GetEnvironmentVariable("CosmosDb_Database");
 
            CosmosClient client = new CosmosClient(GetEnvironmentVariable("CosmosDb_Uri"), GetEnvironmentVariable("CosmosDb_Key"));
            ItemResponse<ResourceHealthDto> response = await client.GetContainer(databaseId, collectionId).CreateItemAsync(alertObj, new PartitionKey(alertObj.resourceId));

            log.LogInformation("Document created in Cosmos: " + response.StatusCode);

            return new OkObjectResult("Resource Health Alert Processed");
        }
        public static string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }
    }
}
