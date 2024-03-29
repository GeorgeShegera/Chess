﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class MatchVsBot : Match
    {
        [JsonProperty("Bot")]
        public Bot Bot { get; set; }

        [JsonProperty("Player")]
        public Player Player { get; set; }

        public MatchVsBot(Bot bot, Player player, Field finalField, TimeSpan time, DateTime date, GameResult result) 
            : base (finalField, time, date, result)
        {
            Bot = bot;
            Player = player;
        }

        public MatchVsBot() 
            : base()
        {

        }

        public override bool ContainsPerson(User person)
        {
            return Player == person;
        }

        public override void Show()
        {
            ShowDate();
            ShowTime();
            ShowSidePlayer(Side.Top);
            FinalField.Show();
            ShowSidePlayer(Side.Bottom);
            switch (GameStatus)
            {
                case GameResult.Checkmate:
                    {
                        if(!Player.Winner)
                        {
                            Console.Write("The bot");
                        }
                        else
                        {
                            Console.Write(Player.Login);
                        }
                        Console.Write("has won by checkmate");
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

        public override void ShowSidePlayer(Side side)
        {
            string result = "";
            for (int i = 0; i < FinalField.Cells.Count + 5; i++)
            {
                result += " ";
            }
            if (Bot.Side == side) result += $"Bot({Bot.Color})";
            else result += $"{Player.Login}({Player.Color})";
            Console.WriteLine(result);
        }
    }
}
