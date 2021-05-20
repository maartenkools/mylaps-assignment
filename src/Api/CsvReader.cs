using Api.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;

namespace Api
{
    public class CsvReader : ICsvReader
    {
        private readonly IFileSystem fileSystem;

        public CsvReader(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async IAsyncEnumerable<string[]> ReadAsync()
        {
            using var stream = this.fileSystem.File.Open("karttimes.csv", FileMode.Open);
            using var streamReader = new StreamReader(stream);
            using var csvReader = new CsvHelper.CsvReader(streamReader, CultureInfo.InvariantCulture);

            csvReader.ReadHeader();
            while (await csvReader.ReadAsync().ConfigureAwait(false))
            {
                yield return new[] { csvReader.GetField<string>("kart"), csvReader.GetField<string>("passingtime") };
            }
        }
    }
}
