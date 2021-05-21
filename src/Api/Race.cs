using Api.Abstractions;
using Api.Model;
using System;
using System.Threading.Tasks;

namespace Api
{
    public class Race : IRace
    {
        private readonly ILaptimeFeed laptimeFeed;
        private readonly IWeatherApi weatherApi;

        public Race(ILaptimeFeed laptimeFeed, IWeatherApi weatherApi)
        {
            this.laptimeFeed = laptimeFeed ?? throw new ArgumentNullException(nameof(laptimeFeed));
            this.weatherApi = weatherApi ?? throw new ArgumentNullException(nameof(weatherApi));
        }

        public Task<CurrentConditions> GetCurrentConditionsAsync() => this.weatherApi.GetCurrentConditionsAsync();

        public async Task<Laptime> StartRaceAsync(uint totalLaps)
        {
            Laptime fastestLaptime = null;

            await foreach (var laptime in this.laptimeFeed.ReadLaptimesAsync().ConfigureAwait(false))
            {
                if (fastestLaptime == null)
                {
                    fastestLaptime = laptime;
                }
                else if (laptime.Time < fastestLaptime.Time)
                {
                    fastestLaptime = laptime;
                }

                // First kart to complete all laps finishes the race
                if (laptime.Lap == totalLaps) return fastestLaptime;
            }

            throw new InvalidOperationException("The laptime feed doesn't provide enough laptimes");
        }
    }
}
