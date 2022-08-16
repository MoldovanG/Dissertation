using DurableSaga.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DurableSaga
{
    public static class Client
    {
        [FunctionName("SagaClient")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string entityGuid = Guid.NewGuid().ToString();
            var entityId = new EntityId(nameof(Booking), entityGuid);
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("SagaOrchestrator", Guid.NewGuid().ToString(), entityGuid);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
