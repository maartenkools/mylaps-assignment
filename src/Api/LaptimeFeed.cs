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
            var lapRegistration = new Dictionary<uint, LapRegistrationRecord>();
            await foreach (var record in this.csvReader.ReadAsync().ConfigureAwait(false))
            {
                var kartNumber = uint.Parse(record[0]);
                var passingTime = TimeSpan.Parse(record[1]);

                var lap = DetermineLap(kartNumber, passingTime, lapRegistration);

                if (lap != null)
                {
                    yield return new Laptime { Number = kartNumber, Lap = lap.Value.LapNumber, Time = lap.Value.Laptime };
                }
            }
        }

        private static (uint LapNumber, TimeSpan Laptime)? DetermineLap(uint kartNumber, TimeSpan passingTime, IDictionary<uint, LapRegistrationRecord> lapRegistration)
        {
            if (!lapRegistration.TryGetValue(kartNumber, out var lapRegistrationRecord))
            {
                lapRegistration[kartNumber] = new LapRegistrationRecord { LapNumber = 0, PassingTime = passingTime };
                return null;
            }

            var lap = lapRegistrationRecord.LapNumber + 1;
            var laptime = passingTime - lapRegistrationRecord.PassingTime;

            lapRegistration[kartNumber] = new LapRegistrationRecord { LapNumber = lap, PassingTime = passingTime };
            return (lap, laptime);
        }

        private class LapRegistrationRecord
        {
            public uint LapNumber { get; set; }
            public TimeSpan PassingTime { get; set; }
        }
    }
}
