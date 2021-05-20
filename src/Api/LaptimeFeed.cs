using Api.Abstractions;
using Api.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api
{
    public class LaptimeFeed : ILaptimeFeed
    {
        private readonly ICsvReader csvReader;

        public LaptimeFeed(ICsvReader csvReader)
        {
            this.csvReader = csvReader ?? throw new ArgumentNullException(nameof(csvReader));
        }

        public async IAsyncEnumerable<Laptime> ReadLaptimesAsync()
        {
            var passingTimeRegistration = new Dictionary<uint, TimeSpan>();
            var lapRegistration = new Dictionary<uint, uint>();
            await foreach (var record in this.csvReader.ReadAsync().ConfigureAwait(false))
            {
                var kartNumber = uint.Parse(record[0]);
                var passingTime = TimeSpan.Parse(record[1]);

                var lap = DetermineLap(kartNumber, lapRegistration);
                var laptime = DetermineLaptime(kartNumber, passingTime, passingTimeRegistration);

                if (lap != 0)
                {
                    yield return new Laptime { Number = kartNumber, Lap = lap, Time = laptime };
                }
            }
        }

        private static uint DetermineLap(uint kartNumber, IDictionary<uint, uint> lapRegistration)
        {
            if(!lapRegistration.TryGetValue(kartNumber, out var currentLap)) currentLap = 0;

            lapRegistration[kartNumber] = currentLap++;

            return currentLap;
        }

        private static TimeSpan DetermineLaptime(uint kartNumber, TimeSpan passingTime, IDictionary<uint, TimeSpan> passingTimeRegistration)
        {
            if(!passingTimeRegistration.TryGetValue(kartNumber, out var previousPassingTime))
            {
                passingTimeRegistration[kartNumber] = passingTime;
                return TimeSpan.Zero;
            }

            return passingTime - previousPassingTime;
        }
    }
}
