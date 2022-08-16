using DurableSaga.Models;
using DurableSaga.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DurableSaga.Activities
{
    public static class Taxi
    {
        [FunctionName("TaxiBookingOrchestrator")]
        public static async Task TaxiBookingOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            var entityGuid = context.GetInput<string>();
            //await Task.Delay(2000);
            // Initial state. (false, false, false)
            var isTaxiBooked = await context.CallActivityAsync<bool>("BookTaxi", null);
            await context.CallEntityAsync(new EntityId(nameof(Booking), entityGuid), "UpdateTaxi", isTaxiBooked);
        }

        [FunctionName("TaxiCancellationOrchestrator")]
        public static async Task TaxiCancellationOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var entityGuid = context.GetInput<string>();

            // Initial state. (false, false, false)
            var isTaxiBooked = await context.CallActivityAsync<bool>("CancelTaxi", null);
            await context.CallEntityAsync(new EntityId(nameof(Booking), entityGuid), "UpdateTaxi", isTaxiBooked);
        }

        [FunctionName("BookTaxi")]
        public static bool BookTaxi([ActivityTrigger] object input, ILogger log)
        {
            var randomFlag = RandomFlagGenerator.Generate();

            log.LogInformation($"Taxi {randomFlag.Message}.");
            return randomFlag.Flag;
        }

        [FunctionName("CancelTaxi")]
        public static bool CancelTaxi([ActivityTrigger] object input, ILogger log)
        {
            log.LogInformation($"Taxi cancelled.");
            return false;
        }
    }
}
