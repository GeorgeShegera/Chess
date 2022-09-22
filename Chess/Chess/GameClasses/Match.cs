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
        [JsonProperty("FinalField")]
        public Field FinalField { get; set; }

        [JsonProperty("Time")]
        public DateTime Time { get; set; }

        [JsonProperty("VsBot")]
        public bool VsBot { get; set; }

        [JsonProperty("FirstPlayer")]
        public Player Winner { get; set; }

        [JsonProperty("SecondPlayer")]
        public Player Loser { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        public Match(Field finallyField, DateTime time, bool vsBot, Player firstPlayer, Player secondPlayer, DateTime date)
        {
            FinalField = finallyField;
            Time = time;
            VsBot = vsBot;
            Winner = firstPlayer;
            Loser = secondPlayer;
            Date = date;
        }
        public Match()
        {
            FinalField = new Field();
            Time = new DateTime();
            VsBot = false;
            Winner = new Player();
            Loser = new Player();
            Date = DateTime.MinValue;
        }
        public void Show()
        {
            Console.WriteLine($"Match Date: {Date}");
            FinalField.Show();
            Console.WriteLine($"The game lasted: {Time}");
            if(VsBot)
            {
                if(!Winner.Authorized())
                {
                    Console.WriteLine($"{Winner.Login} has won.\n" +
                                      $"Bot has lost.");
                }
                else
                {
                    Console.WriteLine($"Bot has won.\n" +
                                      $"{Loser.Login} has lost.");
                }
            }
            else
            {
                Console.WriteLine($"{Winner.Login} has won.\n" +
                                  $"{Loser.Login} has lost");
            }
        }
    }
}
