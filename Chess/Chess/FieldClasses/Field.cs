﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Chess.Game;

namespace Chess
{
    [Serializable]
    public delegate List<Way> FigureWays(Point point, Color color);
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
        public List<Direction> AllDirections()
        {
            return new List<Direction>
            {
                Direction.Left,
                Direction.Right,
                Direction.Up,
                Direction.Down,
                Direction.RightUp,
                Direction.RightDown,
                Direction.LeftDown,
                Direction.LeftUp
            };
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
                        Console.BackgroundColor = ConsoleColor.Yellow;
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

        public List<Way> LegalWays(Color playerColor, Side playerSide)
        {
            List<Way> ways = FindAllWays(playerColor, playerSide);

            return new List<Way>();
        }
        public bool VerifyLegal(Way way, Color playerColor, Side playerSide)
        {
            Color enemyColor = Program.SwitchColor(playerColor);
            Side enemySide = Program.SwitchSide(playerSide);
            List<Way> enWays = FindAllWays(enemyColor, enemySide);
            if (way.SpecialType == SpecialWayType.Castling)
            {
                Direction dir = way.Direction;
                Point curPoint = new Point(way.PrevPlace());
                int limit = Math.Abs(way.NewPlace().CoordX - way.PrevPlace().CoordX);
                for (int i = 0; i <= limit; i++)
                {
                    if (VerPlaceUnderAttack(curPoint, enWays))
                    {
                        return false;
                    }
                    curPoint.MovePoint(dir);
                }
            }
            else
            {

            }
            return true;
        }
        public void MovePiece(Way way)
        {
            if (way.SpecialType == SpecialWayType.Castling)
            {
                Point newKingPoint = new Point(way.NewPlace());
                Point prevPoint;
                Direction dir = way.Direction;
                if (dir == Direction.Left)
                {
                    prevPoint = new Point(0, newKingPoint.CoordY);
                }
                else
                {
                    prevPoint = new Point(Cells.Count - 1, newKingPoint.CoordY);
                }
                Direction oppositeDir = Program.OppositeDirection(dir);
                Point newPoint = new Point(newKingPoint, oppositeDir);
                MovePiece(new Way(CellFigure(newKingPoint), way.PrevPlace(), way.NewPlace(), dir));
                MovePiece(new Way(CellFigure(prevPoint), prevPoint, newPoint, oppositeDir));
            }
            else if (way.SpecialType == SpecialWayType.Enpassant)
            {
                Direction dir;
                if (way.Direction == Direction.LeftUp ||
                    way.Direction == Direction.RightUp)
                {
                    dir = Direction.Down;
                }
                else
                {
                    dir = Direction.Up;
                }
                this[new Point(way.NewPlace(), dir)] =
            }
            else
            {

            }
        }

        public bool VerPlaceUnderAttack(Point point, List<Way> enWays)
        {
            foreach (Way way in enWays)
            {
                if (way.NewPlace() == point)
                {
                    return true;
                }
            }
            return false;
        }

        public List<Way> FindAllWays(Color playerColor, Side playerSide)
        {
            List<Way> result = new List<Way>();
            List<Point> points = FindPiecesPoints(playerColor);
            foreach (Point point in points)
            {
                result.AddRange(FigureWays(point, playerColor, playerSide));
            }
            return result;
        }

        public List<Point> FindPiecesPoints(Color playerColor)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < Cells.Count; i++)
            {
                for (int j = 0; j < Cells[i].Count; j++)
                {
                    Point curPoint = new Point(j, i);
                    if (!EmptyCell(curPoint) &&
                       CellFigure(curPoint).Color == playerColor)
                    {
                        points.Add(curPoint);
                    }
                }
            }
            return points;
        }

        public List<Way> FigureWays(Point point, Color color, Side side)
        {
            if (EmptyCell(point))
            {
                return new List<Way>();
            }
            FigureWays figureWays;
            switch (CellFigure(point).Type)
            {
                case FigureType.Pawn: return FindPawnWays(point, color, side);
                case FigureType.Bishop:
                    {
                        figureWays = FindBishopWays;
                    }
                    break;
                case FigureType.Rook:
                    {
                        figureWays = FindRookWays;
                    }
                    break;
                case FigureType.Queen:
                    {
                        figureWays = FindQueenWays;
                    }
                    break;
                case FigureType.Knight:
                    {
                        figureWays = FindKnightWays;
                    }
                    break;
                case FigureType.King:
                    {
                        figureWays = FindKingWays;
                    }
                    break;
                default:
                    {
                        figureWays = null;
                    }
                    break;
            }
            return figureWays?.Invoke(point, color);
        }

        public bool VerifyNewPlace(Point point, Color color)
        {
            return VerifyPoint(point) &&
                   (EmptyCell(point) ||
                   CellFigure(point).Color != color);
        }

        public void VerifyAttack(Way way, Color color)
        {
            if (!EmptyCell(way.NewPlace()) &&
                CellFigure(way.NewPlace()).Color != color)
            {
                way.AttackWay = true;
            }
        }

        public List<Way> FindKingWays(Point point, Color color)
        {
            List<Way> ways = new List<Way>();
            List<Direction> dirs = AllDirections();
            foreach (Direction dir in dirs)
            {
                Point curPoint = new Point(point);
                curPoint.MovePoint(dir);
                if (VerifyNewPlace(curPoint, color))
                {
                    ways.Add(new Way(CellFigure(point), point, curPoint, dir));
                    VerifyAttack(ways.Last(), color);
                }
            }
            if (!CellFigure(point).FirstMove)
            {
                List<Point> rooksPoints = ChessPiecesPoints(color, FigureType.Rook);
                dirs = new List<Direction>();
                foreach (Point item in rooksPoints)
                {
                    if (CellFigure(item).FirstMove)
                    {
                        if (item.CoordX < Cells.Count / 2)
                        {
                            dirs.Add(Direction.Left);
                        }
                        else
                        {
                            dirs.Add(Direction.Right);
                        }
                    }
                }
                foreach (Direction dir in dirs)
                {
                    Point curPoint = new Point(point);
                    bool castle = false;
                    int limit = 2;
                    if (dir == Direction.Left)
                    {
                        limit = 3;
                    }
                    for (int i = 0; i < limit; i++)
                    {
                        curPoint.MovePoint(dir);
                        if (!EmptyCell(curPoint))
                        {
                            castle = true;
                            break;
                        }
                    }
                    if (castle)
                    {
                        ways.Add(new Way(CellFigure(point), point, curPoint, false, SpecialWayType.Castling, dir));
                    }
                }
            }
            return ways;
        }

        public List<Way> FindKnightWays(Point point, Color color)
        {
            List<Way> ways = new List<Way>();
            List<List<Direction>> dirs = new List<List<Direction>>
            {
                new List<Direction>
                {
                    Direction.Left,
                    Direction.LeftDown
                },
                new List<Direction>
                {
                    Direction.Left,
                    Direction.LeftUp
                },
                new List<Direction>
                {
                    Direction.Right,
                    Direction.RightDown
                },
                new List<Direction>
                {
                    Direction.Right,
                    Direction.RightUp
                },
                new List<Direction>
                {
                    Direction.Up,
                    Direction.RightUp
                },
                new List<Direction>
                {
                    Direction.Up,
                    Direction.LeftUp
                },
                new List<Direction>
                {
                    Direction.Down,
                    Direction.LeftDown
                },
                new List<Direction>
                {
                    Direction.Down,
                    Direction.RightDown
                }
            };
            foreach (List<Direction> directions in dirs)
            {
                Point curPoint = new Point(point);
                foreach (Direction dir in directions)
                {
                    curPoint.MovePoint(dir);
                }
                if (VerifyNewPlace(curPoint, color))
                {
                    ways.Add(new Way(CellFigure(point), point, curPoint, directions.Last()));
                    VerifyAttack(ways.Last(), color);
                }
            }
            return ways;
        }

        public List<Way> FindQueenWays(Point point, Color color)
        {
            List<Direction> diractions = AllDirections();
            return DiractedWays(color, point, diractions);
        }

        public List<Way> FindBishopWays(Point point, Color color)
        {
            List<Direction> diractions = new List<Direction>
            {
                Direction.LeftUp,
                Direction.LeftDown,
                Direction.RightUp,
                Direction.RightDown
            };
            return DiractedWays(color, point, diractions);
        }

        public List<Way> FindRookWays(Point point, Color color)
        {
            List<Direction> dirations = new List<Direction>
            {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right
            };
            return DiractedWays(color, point, dirations);
        }

        public List<Way> FindPawnWays(Point point, Color color, Side side)
        {
            Figure figure = CellFigure(point);
            Point curPoint = new Point(point);
            List<Way> resultWays = new List<Way>();
            Direction verticalDir;
            List<Direction> diagDirs;
            if (side == Side.Bottom)
            {
                verticalDir = Direction.Up;
                diagDirs = new List<Direction>
                {
                    Direction.RightUp,
                    Direction.LeftUp
                };
            }
            else
            {
                verticalDir = Direction.Down;
                diagDirs = new List<Direction>
                {
                    Direction.RightDown,
                    Direction.LeftDown
                };
            }
            curPoint.MovePoint(verticalDir);
            if (VerifyPoint(curPoint) &&
                EmptyCell(curPoint))
            {
                resultWays.Add(new Way(figure, point, curPoint, false, verticalDir));
                curPoint.MovePoint(verticalDir);
                if (figure.FirstMove &&
                    EmptyCell(curPoint))
                {
                    resultWays.Add(new Way(figure, point, curPoint, false, verticalDir));
                }
            }
            foreach (Direction dir in diagDirs)
            {
                curPoint = new Point(point);
                curPoint.MovePoint(dir);
                if (VerifyPoint(curPoint))
                {
                    if (!EmptyCell(curPoint) &&
                        CellFigure(curPoint).Color != color)
                    {
                        resultWays.Add(new Way(figure, point, curPoint, true, dir));
                    }
                    else if (VerifyEnPassant(curPoint, verticalDir))
                    {
                        resultWays.Add(new Way(figure, point, curPoint, true, SpecialWayType.Enpassant, dir));
                    }
                }
            }
            return resultWays;
        }

        public bool VerifyEnPassant(Point point, Direction dir)
        {
            Point prevPoint = new Point(point, dir);
            Point newPoint = new Point(point, Program.OppositeDirection(dir));
            return VerifyPoint(prevPoint) &&
                   VerifyPoint(newPoint) &&
                   this[prevPoint].Track &&
                   this[newPoint].Track &&
                   !EmptyCell(newPoint) &&
                   CellFigure(newPoint).Type == FigureType.Pawn;
        }

        public List<Way> DiractedWays(Color playerColor, Point point, List<Direction> dirs)
        {
            List<Way> ways = new List<Way>();
            foreach (Direction dir in dirs)
            {
                Point curPoint = new Point(point);
                bool endOfPass = false;
                while (!endOfPass)
                {
                    curPoint.MovePoint(dir);
                    if (VerifyNewPlace(curPoint, playerColor))
                    {
                        ways.Add(new Way(CellFigure(point), point, curPoint, dir));
                        VerifyAttack(ways.Last(), playerColor);
                    }
                    else
                    {
                        endOfPass = true;
                    }
                }
            }
            return ways;
        }
        public bool KingInCheck(Color enemyColor, Side enemySide)
        {
            Point point = ChessPiecesPoints(Program.SwitchColor(enemyColor), FigureType.King).First();
            List<Way> enemyWays = FindAllWays(enemyColor, enemySide);
            foreach (Way way in enemyWays)
            {
                if (way.NewPlace() == point)
                {
                    return true;
                }
            }
            return false;
        }
        public List<Point> ChessPiecesPoints(Color color, FigureType figureType)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < Cells.Count; i++)
            {
                for (int j = 0; j < Cells[i].Count; j++)
                {
                    Point curPoint = new Point(j, i);
                    Figure curFigure = CellFigure(curPoint);
                    if (!EmptyCell(curPoint) &&
                        curFigure.Color == color &&
                        curFigure.Type == figureType)
                    {
                        points.Add(new Point(curPoint));
                    }
                }
            }
            return points;
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
