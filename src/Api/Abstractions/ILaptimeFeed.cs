using Api.Model;
using System.Collections.Generic;

namespace Api.Abstractions
{
    public interface ILaptimeFeed
    {
        /// <summary>
        /// Returns an <see cref="IAsyncEnumerable{T}"/> that represents the laptimes of karts going around a track.
        /// </summary>
        IAsyncEnumerable<Laptime> ReadLaptimesAsync();
    }
}
