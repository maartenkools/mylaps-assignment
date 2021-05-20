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
            var previousPassingTimes = new Dictionary<uint, TimeSpan>();
            await foreach (var record in this.csvReader.ReadAsync().ConfigureAwait(false))
            {
                var kartNumber = uint.Parse(record[0]);
                var passingTime = TimeSpan.Parse(record[1]);

                if (!previousPassingTimes.TryGetValue(kartNumber, out var previousPassingTime))
                {
                    previousPassingTimes[kartNumber] = passingTime;
                    yield return new Laptime { Number = kartNumber, Time = TimeSpan.Zero };
                }
                else
                {
                    var laptime = passingTime - previousPassingTime;
                    yield return new Laptime { Number = kartNumber, Time = laptime };
                }
            }
        }
    }
}
