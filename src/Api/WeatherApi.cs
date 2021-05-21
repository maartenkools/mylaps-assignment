using Api.Abstractions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class WeatherApi : IWeatherApi
    {
        private readonly IConfigurationRoot configuration;

        public WeatherApi(IConfigurationRoot configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
    }
}
