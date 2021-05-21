using Api.Abstractions;
using Api.Model;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
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
            var request = new RestRequest("http://dataservice.accuweather.com/currentconditions/v1/249551");
            request.AddQueryParameter("apikey", this.configuration["WeatherApi:Key"]);

            var response = (await this.restClient.GetAsync<List<AccuWeatherResponseDto>>(request).ConfigureAwait(false)).Single();

            return new CurrentConditions
            {
                Description = response.WeatherText,
                Raining = response.HasPrecipitation,
                Temperature = response.Temperature.Metric.Value
            };
        }

        public class AccuWeatherResponseDto
        {
            public string WeatherText { get; set; }
            public bool HasPrecipitation { get; set; }
            public TemperatureDto Temperature { get; set; }

            public class TemperatureDto
            {
                public MetricDto Metric { get; set; }

                public class MetricDto
                {
                    public double Value { get; set; }
                }
            }
        }
    }
}