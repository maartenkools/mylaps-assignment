using Api.Abstractions;
using Api.Model;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.AccuWeather
{
    public class AccuWeatherApi : IWeatherApi
    {
        private readonly IConfigurationRoot configuration;
        private readonly IRestClient restClient;

        public AccuWeatherApi(IConfigurationRoot configuration, IRestClient restClient)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        public async Task<CurrentConditions> GetCurrentConditionsAsync()
        {
            var locationKey = long.Parse(this.configuration["AccuWeather:LocationKey"]);

            var location = await GetCityAsync(locationKey).ConfigureAwait(false);
            var conditions = await GetCurrentConditionsAsync(locationKey).ConfigureAwait(false);

            return new CurrentConditions
            {
                City = location.LocalizedName,
                Description = conditions.WeatherText,
                Raining = conditions.HasPrecipitation,
                Temperature = conditions.Temperature.Metric.Value
            };
        }

        private async Task<AccuWeatherConditionsResponseDto> GetCurrentConditionsAsync(long locationKey)
        {
            var url = this.configuration["AccuWeather:CurrentConditionsUrl"].Replace("{locationKey}", locationKey.ToString());

            var request = new RestRequest(url);
            request.AddQueryParameter("apikey", this.configuration["AccuWeather:ApiKey"]);

            var response = await this.restClient
                                     .GetAsync<List<AccuWeatherConditionsResponseDto>>(request)
                                     .ConfigureAwait(false);

            return response.Single();
        }

        private async Task<AccuWeatherLocationResponseDto> GetCityAsync(long locationKey)
        {
            var url = this.configuration["AccuWeather:LocationsUrl"].Replace("{locationKey}", locationKey.ToString());
            var request = new RestRequest(url);
            request.AddQueryParameter("apikey", this.configuration["AccuWeather:ApiKey"]);

            return await this.restClient
                             .GetAsync<AccuWeatherLocationResponseDto>(request)
                             .ConfigureAwait(false);
        }
    }
}