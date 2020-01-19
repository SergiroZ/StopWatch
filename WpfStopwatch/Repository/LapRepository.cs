using System;
using System.Collections.ObjectModel;
using WpfStopwatch.Entities;

namespace WpfStopwatch.Repository
{
    public class LapRepository
    {
        private ObservableCollection<Lap> laps;

        public ObservableCollection<Lap> Laps =>
            laps ?? (laps = new ObservableCollection<Lap>
            {
                new Lap
                {
                    LapNumber = 0,
                    TSpan = TimeSpan.FromHours(0),
                    TLap = TimeSpan.FromHours(0)
                }
            });
    }
}