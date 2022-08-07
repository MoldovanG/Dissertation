using DurableSaga.Models;
using DurableSaga.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DurableSaga.Activities
{
    public static class Hotel
    {
        [FunctionName("HotelBookingOrchestrator")]
        public static async Task HotelBookingOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
             ILogger logger)
        {
            var entityGuid = context.GetInput<string>();
            var isHotelBooked = await context.CallActivityAsync<bool>("BookHotel", null);
            await context.CallEntityAsync(new EntityId(nameof(Booking), entityGuid), "UpdateHotel", isHotelBooked);
            return;
        }

        [FunctionName("HotelCancellationOrchestrator")]
        public static async Task HotelCancellationOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var entityGuid = context.GetInput<string>();
            var isHotelBooked = await context.CallActivityAsync<bool>("CancelHotel", null);
            await context.CallEntityAsync(new EntityId(nameof(Booking), entityGuid), "UpdateHotel", isHotelBooked);
        }

        [FunctionName("BookHotel")]
        public static bool BookHotel([ActivityTrigger] object input, ILogger log)
        {
            var randomFlag = RandomFlagGenerator.Generate();

            log.LogInformation($"Hotel {randomFlag.Message}.");
            return randomFlag.Flag;
        }

        [FunctionName("CancelHotel")]
        public static bool CancelHotel([ActivityTrigger] object input, ILogger log)
        {
            log.LogInformation($"Hotel cancelled.");
            return false;
        }
    }
}
