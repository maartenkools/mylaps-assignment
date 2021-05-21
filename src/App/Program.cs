using Api;
using Api.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using System;
using System.IO.Abstractions;
using System.Text;
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
            serviceCollection.AddTransient<IRestClient, RestClient>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var race = serviceProvider.GetService<IRace>();
            var winningLap = await race.StartRaceAsync(4).ConfigureAwait(false);

            await PrintCurrentConditionsAsync(race).ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"The race has finished. The winning kart was #{winningLap.Number} with a lap time of {winningLap.Time} on lap #{winningLap.Lap}").ConfigureAwait(false);
        }

        private static async Task PrintCurrentConditionsAsync(IRace race)
        {
            var currentConditions = await race.GetCurrentConditionsAsync().ConfigureAwait(false);

            var sb = new StringBuilder();
            sb.AppendLine("Current conditions in Haarlem");
            sb.Append(currentConditions.Description);
            sb.Append($", {currentConditions.Temperature}C");
            if (currentConditions.Raining) sb.Append(", raining.");
            else sb.Append(", dry.");

            await Console.Out.WriteLineAsync(sb.ToString()).ConfigureAwait(false);
        }
    }
}
