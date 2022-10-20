using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            string json = JsonConvert.SerializeObject(dataBase, settings);
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
                        json += reader.ReadLine();
                    }
                }
            }
            return json;
        }
        public static Color SwitchCol(Color color) => color == Color.White ? Color.Black : Color.White;
        public static Side SwitchSide(Side side) => side == Side.Top ? Side.Bottom : Side.Top;       
        public static Direction OppositeDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                case Direction.RightUp: return Direction.LeftDown;
                case Direction.RightDown: return Direction.LeftUp;
                case Direction.LeftUp: return Direction.RightDown;
                case Direction.LeftDown: return Direction.RightUp;
                default: return new Direction();
            }
        }
        private static User Authorization(DataBase dataBase)
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
                            User result = dataBase.GetPerson(login, password);
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
                            dataBase.AddPerson(new User(login, password));
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
            Console.OutputEncoding = Encoding.UTF8;
            DataBase dataBase = new DataBase();
            string data = LoadDataBase();
            if (data.Length != 0)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                dataBase = JsonConvert.DeserializeObject<DataBase>(data, settings);
            }
            while (true)
            {
                bool endOfSession = false;
                User hostPerson = Authorization(dataBase);
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
                                game.FirstPlayer = new Player(hostPerson);
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
                                                game.AddBot();
                                            }
                                            break;
                                        case LobbyMenu.AddPlayer:
                                            {
                                                if (!game.VerifyAddingPlayer())
                                                {
                                                    Console.WriteLine("The lobby is full");
                                                    break;
                                                }
                                                User newPerson = Authorization(dataBase);
                                                if (newPerson == hostPerson)
                                                {
                                                    Console.WriteLine("You can't play yourself");
                                                    break;
                                                }
                                                game.AddPlayer(newPerson);
                                            }
                                            break;
                                        case LobbyMenu.RemovePlayer:
                                            {
                                                if (game.VsBot)
                                                {
                                                    game.VsBot = false;
                                                    break;
                                                }
                                                if (hostPerson == game.FirstPlayer)
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
                                                dataBase.AddMatch(game.StartGame());
                                                exit = true;
                                                SaveDataBase(dataBase);                                                
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
                                dataBase.ShowTopPersons(5, hostPerson);
                            }
                            break;
                        case Menu.MyMatchHistory:
                            {
                                Console.WriteLine("My match history: ");
                                dataBase.ShowMatchHistory(hostPerson);
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
