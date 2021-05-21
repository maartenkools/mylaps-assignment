using System.Collections.Generic;

namespace Api.Abstractions
{
    public interface ICsvReader
    {
        /// <summary>
        /// Returns an <see cref="IAsyncEnumerable{T}"/> of a string array that represents the lines from a CSV file.
        /// </summary>
        IAsyncEnumerable<string[]> ReadAsync();
    }
}
