using Api.Model;
using System.Threading.Tasks;

namespace Api.Abstractions
{
    public interface IWeatherApi
    {
        /// <summary>
        /// Returns the current conditions of the location specified in the configuration.
        /// </summary>
        /// <returns></returns>
        Task<CurrentConditions> GetCurrentConditionsAsync();
    }
}
