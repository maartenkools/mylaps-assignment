using Api.Model;
using System.Threading.Tasks;

namespace Api.Abstractions
{
    public interface IRace
    {
        /// <summary>
        /// Starts the race and returns the winning <see cref="Laptime"/>.
        /// </summary>
        /// <param name="totalLaps">The number of laps the race should run.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The value of the TResult parameter represents the winning <see cref="Laptime"/>.</returns>
        Task<Laptime> StartRaceAsync(uint totalLaps);
    }
}
