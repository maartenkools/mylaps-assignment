using Api.Abstractions;
using Api.AccuWeather;
using Api.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Net;
using System.Threading;
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

            using (new AssertionScope())
            {
                winningLap.Number.Should().Be(4, "because kart #4 was expected to be the quickest");
                winningLap.Lap.Should().Be(2, "because the winning laptime was the second lap");
                winningLap.Time.Should().Be(TimeSpan.FromSeconds(55));
            }
        }

        [Fact]
        public async Task Race_GetCurrentConditionsAsync_Returns_Current_Conditions()
        {
            var serviceProvider = BuildServiceProvider();

            var race = serviceProvider.GetService<IRace>();

            var currentConditions = await race.GetCurrentConditionsAsync().ConfigureAwait(false);

            using (new AssertionScope())
            {
                currentConditions.City
                                 .Should()
                                 .Be("TestLand");
                currentConditions.Temperature
                                 .Should()
                                 .Be(1.1);
                currentConditions.Raining
                                 .Should()
                                 .BeFalse();
                currentConditions.Description
                                 .Should()
                                 .Be("It's always sunny in Testland!");
            }
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"Resources\karttimes.csv"] = new MockFileData(@"kart,passingtime
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
4,12:02:05
2,12:02:10
1,12:02:12
5,12:02:16
3,12:02:17")
            });

            var locationResponse = new Mock<IRestResponse<AccuWeatherLocationResponseDto>>();
            locationResponse.Setup(x => x.StatusCode)
                            .Returns(HttpStatusCode.OK);
            locationResponse.Setup(x => x.Data)
                            .Returns(() => new AccuWeatherLocationResponseDto { LocalizedName = "TestLand" });

            var conditionsResponse = new Mock<IRestResponse<List<AccuWeatherConditionsResponseDto>>>();
            conditionsResponse.Setup(x => x.StatusCode)
                    .Returns(HttpStatusCode.OK);
            conditionsResponse.Setup(x => x.Data)
                    .Returns(() => new List<AccuWeatherConditionsResponseDto>
                    {
                        new AccuWeatherConditionsResponseDto
                            {
                                HasPrecipitation = false,
                                Temperature = new AccuWeatherConditionsResponseDto.TemperatureDto
                                {
                                    Metric = new AccuWeatherConditionsResponseDto.TemperatureDto.MetricDto
                                    {
                                        Value = 1.1
                                    }
                                },
                                WeatherText = "It's always sunny in Testland!"
                            }
                    });

            var restClient = new Mock<IRestClient>();
            restClient.Setup(x => x.ExecuteGetAsync<AccuWeatherLocationResponseDto>(It.IsAny<IRestRequest>(),
                                                                                    It.IsAny<CancellationToken>()))
                      .ReturnsAsync(() => locationResponse.Object);
            restClient.Setup(x => x.ExecuteGetAsync<List<AccuWeatherConditionsResponseDto>>(It.IsAny<IRestRequest>(),
                                                                                            It.IsAny<CancellationToken>()))
                      .ReturnsAsync(() => conditionsResponse.Object);

            var configuration = new Mock<IConfigurationRoot>();
            configuration.Setup(x => x["AccuWeather:LocationKey"])
                         .Returns("12345");
            configuration.Setup(x => x["AccuWeather:LocationsUrl"])
                         .Returns("https://api.accuweather.com");
            configuration.Setup(x => x["AccuWeather:CurrentConditionsUrl"])
                         .Returns("https://api.accuweather.com");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IRace, Race>();
            serviceCollection.AddTransient<ILaptimeFeed, LaptimeFeed>();
            serviceCollection.AddTransient<ICsvReader, CsvReader>();
            serviceCollection.AddTransient<IFileSystem>(_ => fileSystem);
            serviceCollection.AddTransient<IWeatherApi, AccuWeatherApi>();
            serviceCollection.AddSingleton(configuration.Object);
            serviceCollection.AddTransient(_ => restClient.Object);

            return serviceCollection.BuildServiceProvider(true);
        }
    }
}
