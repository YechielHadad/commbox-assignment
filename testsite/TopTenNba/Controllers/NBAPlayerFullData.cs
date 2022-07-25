using System;

namespace Commbox.Controllers
{
    public class NBAPlayerFullData : NBAPlayer
    {
        public double fgp { get; set; }
        public double ppg { get; set; }
        public double rpg { get; set; }
        public double apg { get; set; }
        public double bpg { get; set; }

        // TODO
        public int CompareTo(NBAPlayerFullData y)
        {
            return 1;
        }
    }
}