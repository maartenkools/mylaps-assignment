using Api.Abstractions;
using Api.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api
{
    public class Race : IRace
    {
        private readonly ILaptimeFeed laptimeFeed;
        private readonly Dictionary<uint, uint> lapCount = new();

        public Race(ILaptimeFeed laptimeFeed)
        {
            this.laptimeFeed = laptimeFeed ?? throw new ArgumentNullException(nameof(laptimeFeed));
        }

        public async Task<Laptime> StartRaceAsync(uint totalLaps)
        {
            Laptime fastestLaptime = null;

            await foreach (var laptime in this.laptimeFeed.ReadLaptimesAsync().ConfigureAwait(false))
            {
                if (!this.lapCount.TryGetValue(laptime.Number, out var currentLap)) currentLap = 0;
                currentLap++;

                if (fastestLaptime == null)
                {
                    fastestLaptime = laptime;
                }
                else if (laptime.Time > fastestLaptime.Time)
                {
                    fastestLaptime = laptime;
                }

                // First kart to complete all laps finishes the race
                if (currentLap == totalLaps) return fastestLaptime;

                this.lapCount[laptime.Number] = currentLap;
            }

            throw new InvalidOperationException("The laptime feed doesn't provide enough laptimes");
        }
    }
}
