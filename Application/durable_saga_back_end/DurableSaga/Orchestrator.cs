using DurableSaga.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DurableSaga
{
    public static class Orchestrator
    {
       [FunctionName("SagaOrchestrator")]
        public static async Task<Booking> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            var entityGuid = context.GetInput<string>();
            // Initial state. (false, false, false)
            var entityId = new EntityId(nameof(Booking), entityGuid);
            var booking = await context.CallEntityAsync<Booking>(entityId, "Get");
            Console.WriteLine("Step 1 - Flight: {0}, Hotel: {1}, Taxi: {2}", booking.Flight, booking.Hotel, booking.Taxi);
            context.SetCustomStatus("Started");
            await context.CallSubOrchestratorAsync<Booking>("HotelBookingOrchestrator", null, entityGuid);
            Console.WriteLine("Step 2 - Flight: {0}, Hotel: {1}, Taxi: {2}", booking.Flight, booking.Hotel, booking.Taxi);
            booking = await context.CallEntityAsync<Booking>(entityId, "Get");
            if (booking.Hotel)
            {
                context.SetCustomStatus("BookedHotel");
                await context.CallSubOrchestratorAsync<Booking>("TaxiBookingOrchestrator", null, entityGuid);
                Console.WriteLine("Step 3 - Flight: {0}, Hotel: {1}, Taxi: {2}", booking.Flight, booking.Hotel, booking.Taxi);
                booking = await context.CallEntityAsync<Booking>(entityId, "Get");
                if (booking.Taxi)
                {
                    context.SetCustomStatus("BookedTaxi");
                    await context.CallSubOrchestratorAsync<Booking>("FlightBookingOrchestrator", null, entityGuid);
                    booking = await context.CallEntityAsync<Booking>(entityId, "Get");
                    Console.WriteLine("Step 4 - Flight: {0}, Hotel: {1}, Taxi: {2}", booking.Flight, booking.Hotel, booking.Taxi);
                }
                else // hotel booked, taxi not booked - cancel hotel
                {
                    context.SetCustomStatus("TaxiFailure");
                    Console.WriteLine("Step 4* - Flight: {0}, Hotel: {1}, Taxi: {2}", booking.Flight, booking.Hotel, booking.Taxi);
                    Console.WriteLine("Cancelling Hotel.");
                    await context.CallSubOrchestratorAsync<Booking>("HotelCancellationOrchestrator", null, entityGuid);
                }

                // hotel booked, taxi booked, flight not booked - cancel hotel & taxi
                if (booking.Hotel && booking.Taxi && !booking.Flight)
                {
                    Console.WriteLine("Step 4** - Flight: {0}, Hotel: {1}, Taxi: {2}", booking.Flight, booking.Hotel, booking.Taxi);
                    Console.WriteLine("Cancelling Hotel and Taxi.");
                    context.SetCustomStatus("FlightFailure");
                    await context.CallSubOrchestratorAsync<Booking>("HotelCancellationOrchestrator", null, entityGuid);
                    await context.CallSubOrchestratorAsync<Booking>("TaxiCancellationOrchestrator", null, entityGuid);
                }
            }

            // This is just to check the final state.
            var completedBooking = await context.CallEntityAsync<Booking>(entityId, "Get");
            if (completedBooking.Flight)
            {
                context.SetCustomStatus("Finished");
            }
            else
            {
                context.SetCustomStatus("Rolled-Back");
            }
            Console.WriteLine("Step 5 - Flight: {0}, Hotel: {1}, Taxi: {2}", completedBooking.Flight, completedBooking.Hotel, completedBooking.Taxi);
            return completedBooking;
        }
    }
}