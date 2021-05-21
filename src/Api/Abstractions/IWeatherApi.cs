using Api.Model;
using System.Threading.Tasks;

namespace Api.Abstractions
{
    public interface IWeatherApi
    {
        /// <summary>
        /// Returns the current conditions in Haarlem, The Netherlands.
        /// </summary>
        /// <returns></returns>
        Task<CurrentConditions> GetCurrentConditionsAsync();
    }
}
