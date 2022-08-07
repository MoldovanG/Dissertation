using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace DurableSaga.Models
{
    public class Booking
    {
        public string Id { get; set; }

        public bool Flight { get; set; }

        public bool Hotel { get; set; }

        public bool Taxi { get; set; }


        public void UpdateFlight(bool isBooked)
        {
            this.Flight = isBooked;
        }

        public void UpdateHotel(bool isBooked)
        {
            this.Hotel = isBooked;
        }

        public void UpdateTaxi(bool isBooked)
        {
            this.Taxi = isBooked;
        }
        public Booking Get() { 
         return this;
        }

        [FunctionName(nameof(Booking))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Booking>();
    }
}
