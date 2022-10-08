using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Game
    {
        [JsonProperty("GameField")]
        public Field GameField { get; set; }

        [JsonProperty("FirstPlayer")]
        public Player FirstPlayer { get; set; }

        [JsonProperty("SecondPlayer")]
        public Player SecondPlayer { get; set; }

        [JsonProperty("VsBot")]
        public bool VsBot { get; set; }

        [JsonProperty("Bot")]
        public Bot Bot { get; set; }


        public Game(Field gameField, Player firstPlayer, Player secondPlayer, bool vsBot, Bot bot)
        {
            GameField = gameField;
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            VsBot = vsBot;
            Bot = bot;
        }
        public Game()
        {
            GameField = new Field();
            FirstPlayer = new Player();
            SecondPlayer = new Player();
            VsBot = false;
            Bot = new Bot();
        }
        public void PlaceCalculation(int length, string myString)
        {
            int lengthOfCell = (length - 1) / 2;
            int result = (lengthOfCell - myString.Length) / 2;
            if (myString.Length % 2 == 0)
            {
                for (int i = 0; i < lengthOfCell; i++)
                {
                    if (i < result)
                    {
                        Console.Write(" ");
                    }
                    else if (i == result)
                    {
                        Console.Write($"{myString}");
                        i += myString.Length;
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
            }
            else
            {
                for (int i = 0; i < lengthOfCell - 1; i++)
                {
                    if (i < result)
                    {
                        Console.Write(" ");
                    }
                    else if (i == result)
                    {
                        Console.Write($"{myString} ");
                        i += myString.Length;
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
            }
        }
        public void ShowLobby(int length, int height)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            PlaceCalculation(2 * (length + 2), "Game Lobby");
            Console.ResetColor();
            Console.WriteLine();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (i == 0 ||
                        i == height - 1 ||
                        i == 4)
                    {
                        Console.Write("-");
                    }
                    else if (j == 0 ||
                        j == length - 1 ||
                        j == Math.Ceiling((double)length / 2) - 1)
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        string curString = "";
                        Player curPlayer;
                        if (j < length / 2)
                        {
                            curPlayer = FirstPlayer;
                        }
                        else
                        {
                            curPlayer = SecondPlayer;
                        }
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        if (curPlayer.Authorized())
                        {
                            if (i == 2)
                            {
                                curString = curPlayer.Login;
                            }
                            else if (i == 7)
                            {
                                curString = curPlayer.Color.ToString();
                            }
                            else if (i == 8)
                            {
                                curString = curPlayer.Side.ToString();
                            }
                            else if (i == 9)
                            {
                                curString = curPlayer.Rating.ToString();
                            }
                        }
                        else if (VsBot)
                        {
                            if (i == 2)
                            {
                                curString = "Bot";
                            }
                            else if (i == 7)
                            {
                                curString = Bot.Color.ToString();
                            }
                            else if (i == 8)
                            {
                                curString = Bot.Side.ToString();
                            }
                        }
                        PlaceCalculation(length, curString);
                        j += (length - 3) / 2 - 1;
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
        }
        public bool VerifyAddingPlayer()
        {
            return !VsBot &&
                   (!FirstPlayer.Authorized() || !SecondPlayer.Authorized());
        }
        public void SwitchColors()
        {
            FirstPlayer.SwitchColor();
            SecondPlayer.SwitchColor();
            if (VsBot)
            {
                Bot.SwitchColor();
            }
        }
        public void SwitchSides()
        {
            FirstPlayer.SwitchSide();
            SecondPlayer.SwitchSide();
            if (VsBot)
            {
                Bot.SwitchSide();
            }
        }
        public void AddPlayer(Player player)
        {
            if (FirstPlayer.Authorized())
            {
                player.Color = Program.SwitchColor(FirstPlayer.Color);
                player.Side = Program.SwitchSide(FirstPlayer.Side);
                SecondPlayer = player;
            }
            else
            {
                player.Color = Program.SwitchColor(SecondPlayer.Color);
                player.Side = Program.SwitchSide(SecondPlayer.Side);
                FirstPlayer = player;
            }
        }
        public void SwapPlaces()
        {
            Player temp = FirstPlayer;
            FirstPlayer = SecondPlayer;
            SecondPlayer = temp;
        }
        public void RemoveFirstPlayer()
        {
            FirstPlayer = new Player();
        }
        public void RemoveSecondPlayer()
        {
            SecondPlayer = new Player();
        }

        public void StartGame(Color playerColor)
        {
            GameField = new Field();
            GameField.Fill(8, 8, playerColor);
            Color curColor = Color.White;
            Side curSide;
            bool checkmate = false;
            bool stalemate = false;
            while (!checkmate || !stalemate)
            {
                if(curColor == FirstPlayer.Color)
                {
                    curSide = FirstPlayer.Side;
                }
                else
                {
                    curSide = SecondPlayer.Side;
                }
                Way way = PlayerTurn(curColor, curSide);
                curColor = Program.SwitchColor(curColor);
                GameField.Clear();
                GameField.MakeTurn(way);
                checkmate = GameField.Checkmate(curColor, curSide);
                stalemate = GameField.Stalemate(curColor, curSide);
            }

        }
        public Way PlayerTurn(Color playerColor, Side side)
        {
            (string, ConsoleColor) msgToPrint = ("", ConsoleColor.Gray);
            while (true)
            {
                Console.Clear();
                if (GameField.KingInCheck(playerColor, side))
                {
                    Point kingPoint = GameField.KingPoint(playerColor, side);
                    GameField[kingPoint].KingInCheck = true;
                    Console.Beep();
                    PrintMessange("Your king is under attack!\n", ConsoleColor.Red);
                }                
                PrintMessange(msgToPrint.Item1, msgToPrint.Item2);
                GameField.Show();
                string introduction;
                if (playerColor == FirstPlayer.Color)
                {
                    introduction = $"Turn of: {FirstPlayer.Login}({playerColor}).\n";
                }
                else
                {
                    introduction = $"Turn of: {SecondPlayer.Login}({playerColor}).\n";
                }
                PrintMessange(introduction, ConsoleColor.Magenta);
                PrintMessange("Choose your chess piece.\n", ConsoleColor.Cyan);
                Point chosenPoint = RequestCoords();
                if (!GameField.VerifyPoint(chosenPoint))
                {
                    Console.Beep();
                    msgToPrint = ("Invalid coords, try again.\n", ConsoleColor.Red);
                    continue;
                }
                if (GameField.EmptyCell(chosenPoint) ||
                   GameField.CellChessPiece(chosenPoint).Color != playerColor)
                {
                    Console.Beep();
                    msgToPrint = ("Here is not your chess piece, try again.\n", ConsoleColor.Red);
                    continue;
                }
                List<Way> legalWays = GameField.LegalPieceWays(chosenPoint, playerColor, side);
                if (legalWays.Count == 0)
                {
                    Console.Beep();
                    msgToPrint = ("The chosen chess piece can't move, chose another chess piece.\n", ConsoleColor.Red);
                    continue;
                }
                GameField.MarkWays(legalWays);
                GameField.Show();
                GameField.ClearWayPoints();
                PrintMessange("Choose a new place.\n", ConsoleColor.Cyan);
                Point newPlace = RequestCoords();
                if (!GameField.VerifyPoint(newPlace))
                {
                    Console.Beep();
                    msgToPrint = ("Invalid coords, try again.\n", ConsoleColor.Red);
                    continue;
                }
                if (legalWays.Any(x => x.NewPlace() == newPlace))
                {
                    // We are selecting elements to be not needed and removing them
                    Way way = legalWays.Except(legalWays.Where(x => x.NewPlace() != newPlace).ToList()).ToList().First();
                    if (GameField.PawnTransform(side, way))
                    {
                        PrintMessange("Choose a new chess piece\n", ConsoleColor.Yellow);
                        Console.WriteLine("1 - Queen\n" +
                                          "2 - Rook\n" +
                                          "3 - Bishop\n" +
                                          "4 - Knight");
                        ChessPieceType type = new ChessPieceType();
                        bool endOfChoosing = false;
                        while (!endOfChoosing)
                        {
                            endOfChoosing = true;
                            PrintMessange("Choose an action: ", ConsoleColor.Green);                            
                            int.TryParse(Console.ReadLine(), out int choice);
                            switch (choice)
                            {
                                case 1:
                                    {
                                        type = ChessPieceType.Queen;
                                    }
                                    break;
                                case 2:
                                    {
                                        type = ChessPieceType.Rook;
                                    }
                                    break;
                                case 3:
                                    {
                                        type = ChessPieceType.Bishop;
                                    }
                                    break;
                                case 4:
                                    {
                                        type = ChessPieceType.Knight;
                                    }
                                    break;
                                default:
                                    {
                                        endOfChoosing = false;
                                        Console.Beep();
                                        PrintMessange("Invalid action, try again.\n", ConsoleColor.Red);
                                    }
                                    break;
                            }
                            GameField[way.PrevPlace()].ChessPiece = new ChessPiece(playerColor, type);
                        }
                    }
                    return way;
                }
                else
                {
                    Console.Beep();
                    msgToPrint = ("You can't move your chess piece like that.\n", ConsoleColor.Red);
                    continue;
                }
            }
        }

        public Point RequestCoords()
        {
            PrintMessange("Input the first coord(letter): ", ConsoleColor.Green);
            char.TryParse(Console.ReadLine(), out char coordX);
            PrintMessange("Input the second coord(number): ", ConsoleColor.Green);
            int.TryParse(Console.ReadLine(), out int coordY);
            return GameField.ConvertCoords(coordY, coordX);
        }

        public void PrintMessange(string message, ConsoleColor foreGroundCol)
        {
            Console.ForegroundColor = foreGroundCol;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}
