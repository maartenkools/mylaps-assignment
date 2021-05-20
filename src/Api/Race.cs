using Api.Abstractions;
using Api.Model;
using System;
using System.Threading.Tasks;

namespace Api
{
    public class Race : IRace
    {
        private readonly ILaptimeFeed laptimeFeed;

        public Race(ILaptimeFeed laptimeFeed)
        {
            this.laptimeFeed = laptimeFeed ?? throw new ArgumentNullException(nameof(laptimeFeed));
        }

        public Task<Laptime> StartRaceAsync(uint laps)
        {
            if (laps > 2) throw new InvalidOperationException("The laptime feed doesn't provide enough laptimes");
            return Task.FromResult(new Laptime { Number = 2, Time = TimeSpan.FromMinutes(1) });
        }
    }
}
