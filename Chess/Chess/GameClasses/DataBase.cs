using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class DataBase
    {
        [JsonProperty("Players")]
        public List<Player> Players { get; set; }

        [JsonProperty("Matches")]
        public List<Match> Matches { get; set; }

        public DataBase(List<Player> players, List<Match> matches)
        {
            Players = players;
            Matches = matches;
        }
    }
}
