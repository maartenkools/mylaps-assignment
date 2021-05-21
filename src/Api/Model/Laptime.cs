using System;

namespace Api.Model
{
    public class Laptime
    {
        public uint Number { get; set; }
        public uint Lap { get; set; }
        public TimeSpan Time { get; set; }
    }
}
