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
        public void ShowTime()
        {
            Console.WriteLine($"The game lasted: {Time.Hours.ToString("00")}:{Time.Minutes.ToString("00")}:" +
                              $"{Time.Seconds.ToString("00")}:{Time.Milliseconds.ToString("000")}");
        }
        public void ShowDate()
        {
            Console.WriteLine($"Match Date: {Date:G}");
        }
        public abstract bool ContainsPerson(Person person);
        public abstract void Show();
        public abstract void ShowSidePlayer(Side side);

    }
}
