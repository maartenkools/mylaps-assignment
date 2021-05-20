using Api.Abstractions;
using Api.Model;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests
{
    public class RaceTests
    {
        [Fact]
        public async Task Race_StartRaceAsync_Throws_InvalidOperationException_If_Missing_Laptimes()
        {
            var serviceProvider = BuildServiceProvider();

            var race = serviceProvider.GetService<IRace>();

            Func<Task<Laptime>> action = () => race.StartRaceAsync(4);

            await action.Should().ThrowAsync<InvalidOperationException>("because the lap time feed doesn't provide enough laps").ConfigureAwait(false);
        }

        [Fact]
        public async Task Race_StartRaceAsync_Returns_Winning_Laptime()
        {
            var serviceProvider = BuildServiceProvider();

            var race = serviceProvider.GetService<IRace>();

            var winningLap = await race.StartRaceAsync(2).ConfigureAwait(false);

            winningLap.Number.Should().Be(2);
            winningLap.Time.Should().Be(TimeSpan.FromMinutes(1));
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                ["karttimes.csv"] = new MockFileData(@"kart,passingtime
1,12:00:00
2,12:00:01
3,12:00:01
4,12:00:02
5,12:00:02
2,12:01:00
1,12:01:10
4,12:01:10
5,12:01:12
3,12:01:15
2,12:02:10
1,12:02:12
4,12:02:12
5,12:02:16
3,12:02:17")
            });

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IRace, Race>();
            serviceCollection.AddTransient<ILaptimeFeed, LaptimeFeed>();
            serviceCollection.AddTransient<ICsvReader, CsvReader>();
            serviceCollection.AddTransient<IFileSystem>(_ => fileSystem ?? new MockFileSystem());

            return serviceCollection.BuildServiceProvider(true);
        }
    }
}
