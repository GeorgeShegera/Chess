using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public DataBase()
        {
            Players = new List<Player>();
            Matches = new List<Match>();
        }

        public Player FindPlayer(string login, string password)
        {
            foreach (Player player in Players)
            {
                if (player.Login == login &&
                    player.Password == password)
                {
                    return player;
                }
            }
            return null;
        }

        public bool ContainsLogin(string login)
        {
            var players = from p in Players
                          where p.Login == login
                          select p;
            return players.Count() != 0;
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }
        public void AddPlayers(List<Player> players)
        {
            Players.AddRange(players);
        }
        public void AddMatch(Match match)
        {
            Matches.Add(match);
        }
        public void ShowTopPlayers(int limit, Player user)
        {
            List<Player> players = Players.Select(x => x).OrderByDescending(x => x.Rating).Take(limit).ToList();
            for (int i = 0; i < limit && i < players.Count; i++)
            {
                Console.WriteLine($"{i + 1}# {players[i].Login} - {players[i].Rating}");
            }
            if (!players.Contains(user))
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    if (Players[i] == user)
                    {
                        Console.WriteLine("...\n" +
                                         $"{i + 1}# {user.Login} - {user.Rating}\n" +
                                         $"...");
                        break;
                    }
                }

            }
        }

        public void ShowMatchHistory(Player player)
        {
            List<Match> matches = (from match in Matches
                                  where match.ContainsPlayer(player)
                                  select match).ToList();
            if (matches.Count != 0) ShowMatchces(matches);
        }

        public void ShowRecentMatches(int limit)
        {
            List<Match> matches = Matches.Select(x => x).Take(limit).ToList();
            if (matches.Count != 0) ShowMatchces(matches);
        }

        public void ShowMatchces(List<Match> matches)
        {
            foreach (var match in matches)
            {
                Console.WriteLine("-------------------------------");
                match.Show();
                Console.WriteLine();
            }
            Console.WriteLine("-------------------------------");
        }
    }
}
