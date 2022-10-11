using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class MatchVsPlayer : Match
    {
        [JsonProperty("FirstPlayer")]
        public Player FirstPlayer { get; set; }

        [JsonProperty("SecondPlayer")]
        public Player SecondPlayer { get; set; }

        public MatchVsPlayer(Player winner, Player loser, Field finalField, TimeSpan time, DateTime date, GameResult result)
            : base(finalField, time, date, result)
        {
            FirstPlayer = winner;
            SecondPlayer = loser;
        }

        public override bool ContainsPerson(Person person)
        {
            return FirstPlayer == person || SecondPlayer == person;
        }

        public override void Show()
        {
            ShowDate();
            ShowTime();
            ShowSidePlayer(Side.Top);
            FinalField.Show();
            ShowSidePlayer(Side.Bottom);
            switch (Result)
            {
                case GameResult.Checkmate:
                    {
                        Console.Write($"{GetWinner().Login} has won by checkmate\n" +
                                          $"Rating change: {GetWinner().Color} +30, " +
                                          $"{GetLoser().Color} -30");
                    }
                    break;
                case GameResult.StaleMate:
                    {
                        Console.Write("Game drawn by stalemate");
                    }
                    break;
                case GameResult.FiftyMoveRule:
                    {
                        Console.Write("Game drawn by Fifty-move rule");
                    }
                    break;
            }
        }
        public Player GetLoser()
        {
            if (FirstPlayer.Winner) return SecondPlayer;
            else return FirstPlayer;
        }
        public Player GetWinner()
        {
            if (FirstPlayer.Winner) return FirstPlayer;
            else return SecondPlayer;
        }
        public override void ShowSidePlayer(Side side)
        {
            string result = "";
            for (int i = 0; i < FinalField.Cells.Count + 5; i++)
            {
                result += " ";
            }
            if (FirstPlayer.Side == side) result += $"{FirstPlayer.Login}({FirstPlayer.Color})";
            else result += $"{SecondPlayer.Login}({SecondPlayer.Color})";
            Console.WriteLine(result);
        }
    }
}
