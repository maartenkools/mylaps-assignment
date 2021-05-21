namespace Api.AccuWeather
{
    public class AccuWeatherConditionsResponseDto
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