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
            await foreach (var record in this.csvReader.ReadAsync().ConfigureAwait(false))
            {
                yield return new Laptime { Number = uint.Parse(record[0]), Time = TimeSpan.Parse(record[1]) };
            }
        }
    }
}
