using Api;
using Api.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace App
{
    internal static class Program
    {
        internal static async Task Main()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IRace, Race>();
            serviceCollection.AddTransient<ILaptimeFeed, LaptimeFeed>();
            serviceCollection.AddTransient<ICsvReader, CsvReader>();
            serviceCollection.AddTransient<IFileSystem, FileSystem>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var race = serviceProvider.GetService<IRace>();
            var winningLap = await race.StartRaceAsync(4).ConfigureAwait(false);

            await Console.Out.WriteLineAsync($"The race has finished. The winning kart was #{winningLap.Number} with a lap time of {winningLap.Time} on lap #{winningLap.Lap}").ConfigureAwait(false);
        }
    }
}
