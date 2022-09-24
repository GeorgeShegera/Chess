using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    internal class Program
    {
        public static void SaveDataBase(DataBase dataBase)
        {
            string json = JsonConvert.SerializeObject(dataBase);
            using (Stream stream = new FileStream("chess.json", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine(json);
                }
            }
        }

        public static string LoadDataBase()
        {
            string json = "";
            using (Stream stream = new FileStream("chess.json", FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        json = reader.ReadLine();
                    }
                }
            }
            return json;
        }
        public static Color SwitchColor(Color color)
        {
            if (color == Color.White)
            {
                return Color.Black;
            }
            else
            {
                return Color.White;
            }
        }
        public static Side SwitchSide(Side side)
        {
            if (side == Side.Top)
            {
                return Side.Bottom;
            }
            else
            {
                return Side.Top;
            }
        }
        private static Player Authorization(DataBase dataBase)
        {
            string login;
            string password;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1 - Sign in\n" +
                                  "2 - Sign on");
                int.TryParse(Console.ReadLine(), out int action);
                switch (action)
                {
                    case 1:
                        {
                            Console.Write("Input your login: ");
                            login = Console.ReadLine();
                            Console.Write("Input your password: ");
                            password = Console.ReadLine();
                            Player result = dataBase.FindPlayer(login, password);
                            if (result is null)
                            {
                                Console.WriteLine("Invalid login or password");
                            }
                            else
                            {
                                return result;
                            }
                        }
                        break;
                    case 2:
                        {
                            Console.Write("Input your login: ");
                            login = Console.ReadLine();
                            if (dataBase.ContainsLogin(login))
                            {
                                Console.WriteLine("This login is taken");
                                break;
                            }
                            Console.Write("Input your password: ");
                            password = Console.ReadLine();
                            dataBase.AddPlayer(new Player(login, password));
                            SaveDataBase(dataBase);
                            Console.WriteLine("New account has been added");
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Invalid mode");
                        }
                        break;
                }
                Console.ReadKey();
            }
        }

        static void Main(string[] args)
        {
            DataBase dataBase = new DataBase();
            string data = LoadDataBase();
            if (data.Length != 0)
            {
                dataBase = JsonConvert.DeserializeObject<DataBase>(data);
            }
            while (true)
            {
                bool endOfSession = false;
                Player curPlayer = Authorization(dataBase);
                while (!endOfSession)
                {
                    Console.Clear();
                    Console.WriteLine($"{(int)Menu.Rules} - Rules\n" +
                                      $"{(int)Menu.CreateLobby} - Create Lobby\n" +
                                      $"{(int)Menu.Rating} - Rating\n" +
                                      $"{(int)Menu.MyMatchHistory} - My match history\n" +
                                      $"{(int)Menu.RecentMatches} - Recent matches\n" +
                                      $"{(int)Menu.Exit} - Exit");
                    Enum.TryParse(Console.ReadLine(), out Menu menu);
                    switch (menu)
                    {
                        case Menu.Rules:
                            {
                                Console.WriteLine("Chess is a two-player abstract strategy board game.\n" +
                                    "Each player controls sixteen pieces of six types on a chessboard.\n" +
                                    "Each type of piece moves in a distinct way\n" +
                                    "The object of the game is to checkmate (inescapably threaten with capture) the opponent's king.\n" +
                                    "A game can end in various ways besides checkmate: a player can resign,\n" +
                                    "and there are several ways in which a game can end in a draw.");
                            }
                            break;
                        case Menu.CreateLobby:
                            {
                                Game game = new Game();
                                game.FirstPlayer = curPlayer;
                                bool exit = false;
                                while (!exit)
                                {
                                    Console.Clear();
                                    game.ShowLobby(41, 13);
                                    Console.WriteLine($"{(int)LobbyMenu.AddBot} - Add bot\n" +
                                                      $"{(int)LobbyMenu.AddPlayer} - Add player\n" +
                                                      $"{(int)LobbyMenu.RemovePlayer} - Remove player\n" +
                                                      $"{(int)LobbyMenu.StartGame} - Start game\n" +
                                                      $"{(int)LobbyMenu.SwitchColor} - Switch color\n" +
                                                      $"{(int)LobbyMenu.SwitchSide} - Switch side\n" +
                                                      $"{(int)LobbyMenu.SwapPlaces} - Swap places\n" +
                                                      $"{(int)LobbyMenu.Exit} - Exit");
                                    Enum.TryParse(Console.ReadLine(), out LobbyMenu lobbyMenu);
                                    switch (lobbyMenu)
                                    {
                                        case LobbyMenu.AddBot:
                                            {
                                                if (!game.VerifyAddingPlayer())
                                                {
                                                    Console.WriteLine("The lobby is full");
                                                    break;
                                                }
                                                game.VsBot = true;
                                                game.Bot = new Bot(SwitchColor(curPlayer.Color), SwitchSide(curPlayer.Side));
                                            }
                                            break;
                                        case LobbyMenu.AddPlayer:
                                            {
                                                if (!game.VerifyAddingPlayer())
                                                {
                                                    Console.WriteLine("The lobby is full");
                                                    break;
                                                }
                                                Player player = Authorization(dataBase);
                                                if (player == curPlayer)
                                                {
                                                    Console.WriteLine("You can't play yourself");
                                                    break;
                                                }
                                                game.AddPlayer(player);
                                            }
                                            break;
                                        case LobbyMenu.RemovePlayer:
                                            {
                                                if (game.VsBot)
                                                {
                                                    game.VsBot = false;
                                                    break;
                                                }
                                                if (curPlayer == game.FirstPlayer)
                                                {
                                                    game.RemoveSecondPlayer();
                                                }
                                                else
                                                {
                                                    game.RemoveFirstPlayer();
                                                }
                                            }
                                            break;
                                        case LobbyMenu.StartGame:
                                            {
                                                if (!game.VsBot &&
                                                   (!game.FirstPlayer.Authorized() ||
                                                    !game.SecondPlayer.Authorized()))
                                                {
                                                    Console.WriteLine("There are not enough players");
                                                    break;
                                                }
                                                game.StartGame(curPlayer.Color);
                                            }
                                            break;
                                        case LobbyMenu.SwitchColor:
                                            {
                                                game.SwitchColors();
                                            }
                                            break;
                                        case LobbyMenu.SwitchSide:
                                            {
                                                game.SwitchSides();
                                            }
                                            break;
                                        case LobbyMenu.SwapPlaces:
                                            {
                                                game.SwapPlaces();
                                            }
                                            break;
                                        case LobbyMenu.Exit:
                                            {
                                                game = new Game();
                                                exit = true;
                                            }
                                            break;
                                        default:
                                            {
                                                Console.WriteLine("Invalid mode");
                                            }
                                            break;
                                    }
                                    Console.ReadKey();
                                }
                            }
                            break;
                        case Menu.Rating:
                            {
                                dataBase.ShowTopPlayers(5, curPlayer);
                            }
                            break;
                        case Menu.MyMatchHistory:
                            {
                                Console.WriteLine("My match history: ");
                                dataBase.ShowMatchHistory(curPlayer);
                            }
                            break;
                        case Menu.RecentMatches:
                            {
                                Console.WriteLine("Recent Matches: ");
                                dataBase.ShowRecentMatches(5);
                            }
                            break;
                        case Menu.Exit:
                            {
                                endOfSession = true;
                            }
                            break;
                        default:
                            {
                                Console.WriteLine("Invalid mode");
                            }
                            break;
                    }
                    Console.ReadKey();
                }
            }
        }
    }
}
