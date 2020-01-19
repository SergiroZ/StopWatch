using System;

namespace WpfStopwatch.Entities
{
    public class Lap
    {
        public int LapNumber { get; set; }

        public TimeSpan TSpan { get; set; }

        public TimeSpan TLap { get; set; }
    }
}