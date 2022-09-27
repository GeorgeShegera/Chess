using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Chess.Game;

namespace Chess
{
    [Serializable]
    public class Field
    {
        [JsonProperty("Cells")]
        public List<List<Cell>> Cells { get; set; }

        public Field(List<List<Cell>> cells)
        {
            Cells = cells;
        }
        public Field()
        {
            Cells = new List<List<Cell>>();
        }
        public Color SwitchColor(Color color)
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

        public void Fill(int length, int height, Color color)
        {
            Color curCellCol = Program.SwitchColor(color);
            for (int i = 0; i < height; i++)
            {
                Cells.Add(new List<Cell>());
                for (int j = 0; j < length; j++)
                {
                    FigureType curType = new FigureType();
                    bool isEmpty = true;
                    if (i <= 1 || i >= height - 2)
                    {
                        isEmpty = false;
                        if (i == 1 || i == height - 2)
                        {
                            curType = FigureType.Pawn;
                        }
                        else if (j == 0 || j == length - 1)
                        {
                            curType = FigureType.Rook;
                        }
                        else if (j == 1 || j == length - 2)
                        {
                            curType = FigureType.Knight;
                        }
                        else if (j == 2 || j == length - 3)
                        {
                            curType = FigureType.Bishop;
                        }
                        else if (j == length / 2)
                        {
                            curType = FigureType.King;
                        }
                        else
                        {
                            curType = FigureType.Queen;
                        }
                    }
                    Figure figure = new Figure(color, curType, true);
                    Cells[i].Add(new Cell(curCellCol, figure, isEmpty, false));
                    curCellCol = Program.SwitchColor(curCellCol);
                }
                curCellCol = Program.SwitchColor(curCellCol);
                if (i == height / 2)
                {
                    color = Program.SwitchColor(color);
                }
            }
        }

        public void Show()
        {
            Console.WriteLine("    A  B  C  D  E  F  G  H  ");
            Console.WriteLine("  --------------------------");
            for (int i = 0; i < Cells.Count; i++)
            {
                Console.Write(Cells.Count - i + " |");
                for (int j = 0; j < Cells[i].Count; j++)
                {
                    if (Cells[i][j].Track)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (Cells[i][j].Color == Color.White)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    }
                    if (!Cells[i][j].IsEmpty)
                    {
                        if (Cells[i][j].Figure.Color == Color.White)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        FigureType type = Cells[i][j].Figure.Type;
                        if (type == FigureType.Pawn)
                        {
                            Console.Write(" P ");
                        }
                        else if (type == FigureType.Rook)
                        {
                            Console.Write(" R ");
                        }
                        else if (type == FigureType.Knight)
                        {
                            Console.Write(" N ");
                        }
                        else if (type == FigureType.Bishop)
                        {
                            Console.Write(" B ");
                        }
                        else if (type == FigureType.King)
                        {
                            Console.Write(" K ");
                        }
                        else
                        {
                            Console.Write(" Q ");
                        }
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine("|" + $" {Cells.Count - i}");
            }
            Console.WriteLine("  --------------------------\n" +
                              "    A  B  C  D  E  F  G  H  \n");
        }

        public bool VerifyPoint(Point point)
        {
            return point.CoordX >= 0 && point.CoordY >= 0 &&
                   point.CoordX < Cells.Count &&
                   point.CoordY < Cells.Count;
        }
        public bool EmptyCell(Point point)
        {
            return this[point].IsEmpty;
        }
        public Figure CellFigure(Point point)
        {
            return this[point].Figure;
        }
        public Point ConvertCoords(int coordY, char coordX)
        {
            coordX = Char.ToLower(coordX);
            return new Point(coordX - 'a', Cells.Count - coordY);
        }
        public List<Way> FindWays(Point point, Color color, Side side)
        {
            if (EmptyCell(point))
            {
                return new List<Way>();
            }
            switch (CellFigure(point).Type)
            {
                case FigureType.Pawn: return FindPawnWays(point, color, side);
                case FigureType.Bishop: return FindBishopWays(point, color);
                case FigureType.Rook: return FindRookWays(point, color);


                default: return new List<Way>();
            }
        }
        public List<Way> FindBishopWays(Point point, Color color)
        {
            List<Diraction> diractions = new List<Diraction>
            {
                Diraction.LeftUp,
                Diraction.LeftDown,
                Diraction.RightUp,
                Diraction.RightDown
            };
            return DiractedWays(color, point, diractions);
        }
        public List<Way> FindRookWays(Point point, Color color)
        {
            List<Diraction> dirations = new List<Diraction>
            {
                Diraction.Up,
                Diraction.Down,
                Diraction.Left,
                Diraction.Right
            };
            return DiractedWays(color, point, dirations);
        }
        public List<Way> FindPawnWays(Point point, Color color, Side side)
        {
            Figure figure = CellFigure(point);
            Point curPoint = new Point(point);
            List<Way> resultWays = new List<Way>();
            Diraction verticalDir;
            List<Diraction> diagDirs;
            if (side == Side.Bottom)
            {
                verticalDir = Diraction.Up;
                diagDirs = new List<Diraction>
                {
                    Diraction.RightUp,
                    Diraction.LeftUp
                };
            }
            else
            {
                verticalDir = Diraction.Down;
                diagDirs = new List<Diraction>
                {
                    Diraction.RightDown,
                    Diraction.LeftDown
                };
            }
            curPoint.MovePoint(verticalDir);
            if (VerifyPoint(curPoint) &&
                EmptyCell(curPoint))
            {
                resultWays.Add(new Way(figure, point, curPoint, false));
                curPoint.MovePoint(verticalDir);
                if (figure.FirstMove &&
                    EmptyCell(curPoint))
                {
                    resultWays.Add(new Way(figure, point, curPoint, false));
                }
            }            
            foreach(Diraction dir in diagDirs)
            {
                curPoint = new Point(point);
                curPoint.MovePoint(dir);
                if (!EmptyCell(curPoint) &&
                    CellFigure(curPoint).Color != color)
                {
                    resultWays.Add(new Way(figure, point, curPoint, true));
                }
            }
            return resultWays;
        }
        public List<Way> DiractedWays(Color playerColor, Point point, List<Diraction> dirs)
        {            
            List<Way> ways = new List<Way>();
            Figure figure = CellFigure(point);
            foreach (Diraction dir in dirs)
            {
                Point curPoint = new Point(point);
                bool endOfPass = false;
                while (!endOfPass)
                {
                    curPoint.MovePoint(dir);
                    bool attackWay = false;
                    if (!VerifyPoint(curPoint))
                    {
                        break;
                    }
                    if (!EmptyCell(curPoint))
                    {
                        if (CellFigure(curPoint).Color != playerColor)
                        {
                            attackWay = true;
                            endOfPass = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                    ways.Add(new Way(figure, point, curPoint, attackWay));
                }
            }
            return ways;
        }
        public Cell this[Point point]
        {
            get
            {
                return Cells[point.CoordY][point.CoordX];
            }
            set
            {
                Cells[point.CoordY][point.CoordX] = value;
            }
        }

    }
}
