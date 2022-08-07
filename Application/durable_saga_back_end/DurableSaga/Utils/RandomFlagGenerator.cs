using DurableSaga.Models;
using System;

namespace DurableSaga.Utils
{
    public class RandomFlagGenerator
    {
        public static RandomFlag Generate(int chance = 8)
        {
            var random = new Random();
            var num = random.Next(1, 10);
            var flag = num % chance == 0 ? false : true;
            var msg = flag ? "booked" : "not booked";

            return new RandomFlag { Flag = flag, Message = msg };
        }
    }
}
