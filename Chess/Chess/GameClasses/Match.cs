using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Match
    {
        [JsonProperty("FinallyField")]
        public Field FinallyField { get; set; }

        [JsonProperty("Time")]
        public DateTime Time { get; set; }

        [JsonProperty("VsBot")]
        public bool VsBot { get; set; }

        [JsonProperty("FirstPlayer")]
        public Player FirstPlayer { get; set; }

        [JsonProperty("SecondPlayer")]
        public Player SecondPlayer { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        public Match(Field finallyField, DateTime time, bool vsBot, Player firstPlayer, Player secondPlayer, DateTime date)
        {
            FinallyField = finallyField;
            Time = time;
            VsBot = vsBot;
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            Date = date;
        }
    }
}
