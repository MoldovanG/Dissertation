using DurableSaga.Models;
using DurableSaga.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DurableSaga.Activities
{
    public static class Flight
    {
        [FunctionName("FlightBookingOrchestrator")]
        public static async Task FlightBookingOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
             ILogger logger)
        {
            var entityGuid = context.GetInput<string>();
            // Initial state. (false, false, false)
            var isFlightBooked = await context.CallActivityAsync<bool>("BookFlight", null);
            await context.CallEntityAsync(new EntityId(nameof(Booking), entityGuid), "UpdateFlight", isFlightBooked);
        }

        [FunctionName("BookFlight")]
        public static bool BookFlight([ActivityTrigger] string id, ILogger log)
        {
            var randomFlag = RandomFlagGenerator.Generate(2);

            log.LogInformation($"Flight {randomFlag.Message}.");
            return randomFlag.Flag;
        }

        [FunctionName("CancelFlight")]
        public static bool CancelFlight([ActivityTrigger] object input, ILogger log)
        {
            log.LogInformation($"Flight cancelled.");
            return false;
        }
    }
}
