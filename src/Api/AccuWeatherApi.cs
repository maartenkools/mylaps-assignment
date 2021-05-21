using Api.Abstractions;
using Api.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Api
{
    public class AccuWeatherApi : IWeatherApi
    {
        private readonly IConfigurationRoot configuration;

        public AccuWeatherApi(IConfigurationRoot configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task<CurrentConditions> GetCurrentConditionsAsync() => throw new NotImplementedException();
    }
}
