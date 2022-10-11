using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]    
    public abstract class Match
    {
        [JsonProperty("FinalField")]
        public Field FinalField { get; set; }

        [JsonProperty("Time")]
        public TimeSpan Time { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("GameResult")]
        public GameResult Result { get; set; }

        public Match(Field finallyField, TimeSpan time, DateTime date, GameResult result)
        {
            FinalField = finallyField;
            Time = time;
            Date = date;
            Result = result;
        }
        public Match()
        {
            FinalField = new Field();
            Time = new TimeSpan();
            Date = DateTime.MinValue;
            Result = new GameResult();
        }
        public abstract bool ContainsPlayer(Player player);
        public abstract void Show();
        public abstract void ShowSidePlayer(Side side);

    }
}
