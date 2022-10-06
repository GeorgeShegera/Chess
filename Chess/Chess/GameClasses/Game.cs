using System;
using System.Collections.Generic;
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
            while (true)
            {
                Way way = PlayerTurn(curColor);
                curColor = Program.SwitchColor(curColor);
                GameField.MakeTurn(way);
            }

        }
        public Way PlayerTurn(Color playerColor)
        {
            (string, ConsoleColor) msgToPrint = ("", ConsoleColor.Gray);
            Side curSide;
            while (true)
            {
                Console.Clear();
                PrintMessange(msgToPrint.Item1, msgToPrint.Item2);
                GameField.Show();
                string introduction;
                if (playerColor == FirstPlayer.Color)
                {
                    introduction = $"Turn of: {FirstPlayer.Login}({playerColor}).\n";
                    curSide = FirstPlayer.Side;
                }
                else
                {
                    introduction = $"Turn of: {SecondPlayer.Login}({playerColor}).\n";
                    curSide = SecondPlayer.Side;
                }
                PrintMessange(introduction, ConsoleColor.Magenta);
                if (GameField.KingInCheck(playerColor, curSide))
                {
                    PrintMessange("Your king is under attack!\n", ConsoleColor.Blue);
                }
                PrintMessange("Choose your chess piece.\n", ConsoleColor.Cyan);
                Point chosenPoint = RequestCoords();
                if(!GameField.VerifyPoint(chosenPoint))
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
                List<Way> legalWays = GameField.LegalPieceWays(chosenPoint, playerColor, curSide);
                if (legalWays.Count == 0)
                {
                    Console.Beep();
                    msgToPrint = ("The chosen chess piece can't move, chose another chess pice.\n", ConsoleColor.Red);
                    continue;
                }
                GameField.MarkWays(legalWays);
                GameField.Show();
                GameField.Clear();
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
