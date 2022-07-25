using System;
using System.Collections;
using System.Collections.Generic;

namespace Commbox.Controllers
{
    public class NBAPlayer 
    {
        public string playerId { get; set; }
        public string playerFullName { get; set; }
        public string playerHeightInMeters { get; set; }
        public string playerPosition { get; set; }
        public string playerDateOfBirth { get; set; }
        public NBATeam team { get; set; }
    }


    public class NBAPlayerEqualityComparer : IEqualityComparer<NBAPlayer>
    {
        public bool Equals(NBAPlayer x, NBAPlayer y)
        {
            return x.playerId == y.playerId;
        }

        public int GetHashCode(NBAPlayer obj)
        {
            return obj.playerId.GetHashCode();
        }
    }
}