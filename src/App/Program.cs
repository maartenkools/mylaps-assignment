using Api;
using Api.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace App
{
    internal class Program
    {
        internal static async Task Main()
        {
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<Program>();
            var configurationRoot = builder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IRace, Race>();
            serviceCollection.AddTransient<ILaptimeFeed, LaptimeFeed>();
            serviceCollection.AddTransient<ICsvReader, CsvReader>();
            serviceCollection.AddTransient<IFileSystem, FileSystem>();
            serviceCollection.AddTransient<IWeatherApi, AccuWeatherApi>();
            serviceCollection.AddSingleton(configurationRoot);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var race = serviceProvider.GetService<IRace>();
            var winningLap = await race.StartRaceAsync(4).ConfigureAwait(false);

            await Console.Out.WriteLineAsync($"The race has finished. The winning kart was #{winningLap.Number} with a lap time of {winningLap.Time} on lap #{winningLap.Lap}").ConfigureAwait(false);
        }
    }
}
