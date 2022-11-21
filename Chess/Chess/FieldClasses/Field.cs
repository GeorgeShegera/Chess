using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Chess.Game;

namespace Chess
{
    [Serializable]
    public delegate List<Way> PieceWays(Point point, Color color, bool guard);
    public class Field
    {
        [JsonProperty("Cells")]
        public List<List<Cell>> Cells { get; set; }

        [JsonProperty("WhitePieces")]
        private List<Point> WhitePieces { get; set; }

        [JsonProperty("BlackPieces")]
        private List<Point> BlackPieces { get; set; }

        private Point WhiteKingPoint { get; set; }

        private Point BlackKingPoint { get; set; }

        public Field(List<List<Cell>> cells)
        {
            Cells = cells;
        }
        public Field()
        {
            Cells = new List<List<Cell>>();
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
        public void UpdatePiecesPoints()
        {
            BlackPieces = new List<Point>();
            WhitePieces = new List<Point>();
            Point curPoint;
            for (int i = 0; i < Cells.Count; i++)
            {
                for (int j = 0; j < Cells[i].Count; j++)
                {
                    curPoint = new Point(j, i);
                    if (!EmptyCell(curPoint))
                    {
                        if (GetCellPiece(curPoint).Color == Color.White) WhitePieces.Add(curPoint);
                        else BlackPieces.Add(curPoint);
                    }
                }
            }
        }
        public void CheckPieces()
        {
            List<Point> pieces = new List<Point>(WhitePieces);
            foreach (Point point in pieces)
            {
                if (EmptyCell(point) || 
                    GetCellPiece(point).Color != Color.White)
                {
                    WhitePieces.Remove(point);
                }
            }
            pieces = new List<Point>(BlackPieces);
            foreach (Point point in pieces)
            {
                if (EmptyCell(point) ||
                    GetCellPiece(point).Color != Color.Black)
                {
                    BlackPieces.Remove(point);
                }
            }
        }
        public void AddPiecePoint(Point point, Color color)
        {
            if (color == Color.White) WhitePieces.Add(point);
            else BlackPieces.Add(point);
        }
        public void RemovePiecePoint(Point point, Color color)
        {
            List<Point> points = color == Color.White ? WhitePieces : BlackPieces;
            foreach(Point p in points)
            {
                if(p == point)
                {
                    points.Remove(p);
                    return;
                }
            }
        }
        public void Fill(int length, int height, Color piecesColor)
        {
            if (piecesColor == Color.White)
            {
                WhiteKingPoint = new Point(3, 0);
                BlackKingPoint = new Point(3, 7);
            }
            else
            {
                BlackKingPoint = new Point(4, 0);
                WhiteKingPoint = new Point(4, 7);
            }
            Color curCellCol = Color.White;
            for (int i = 0; i < height; i++)
            {
                Cells.Add(new List<Cell>());
                for (int j = 0; j < length; j++)
                {
                    PieceType curType = new PieceType();
                    bool isEmpty = true;
                    if (i <= 1 || i >= height - 2)
                    {
                        isEmpty = false;
                        if (i == 1 || i == height - 2)
                        {
                            curType = Chess.PieceType.Pawn;
                        }
                        else if (j == 0 || j == length - 1)
                        {
                            curType = Chess.PieceType.Rook;
                        }
                        else if (j == 1 || j == length - 2)
                        {
                            curType = Chess.PieceType.Knight;
                        }
                        else if (j == 2 || j == length - 3)
                        {
                            curType = Chess.PieceType.Bishop;
                        }
                        else if (curCellCol != piecesColor)
                        {
                            curType = Chess.PieceType.King;
                        }
                        else
                        {
                            curType = Chess.PieceType.Queen;
                        }
                    }
                    ChessPiece figure = new ChessPiece(piecesColor, curType, new int());
                    Cells[i].Add(new Cell(curCellCol, figure, isEmpty, false));
                    curCellCol = Program.SwitchCol(curCellCol);
                }
                curCellCol = Program.SwitchCol(curCellCol);
                if (i == height / 2)
                {
                    piecesColor = Program.SwitchCol(piecesColor);
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
                    if (Cells[i][j].KingInCheck)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    else if (Cells[i][j].ChosenPoint)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                    }
                    else if (Cells[i][j].Track)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
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
                        if (Cells[i][j].WayPoint)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("(");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                        if (Cells[i][j].Piece.Color == Color.White)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        PieceType type = Cells[i][j].Piece.Type;
                        if (type == Chess.PieceType.Pawn)
                        {
                            Console.Write("P");
                        }
                        else if (type == Chess.PieceType.Rook)
                        {
                            Console.Write("R");
                        }
                        else if (type == Chess.PieceType.Knight)
                        {
                            Console.Write("N");
                        }
                        else if (type == Chess.PieceType.Bishop)
                        {
                            Console.Write("B");
                        }
                        else if (type == Chess.PieceType.King)
                        {
                            Console.Write("K");
                        }
                        else if (type == Chess.PieceType.Queen)
                        {
                            Console.Write("Q");
                        }
                        if (Cells[i][j].WayPoint)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(")");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                    else if (Cells[i][j].WayPoint)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" • ");
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
                              "    A  B  C  D  E  F  G  H  ");
        }
        public void MarkWays(List<Way> ways)
        {
            this[ways.First().Start()].ChosenPoint = true;
            foreach (Way way in ways)
            {
                this[way.End()].WayPoint = true;
            }
        }
        public bool VerifyPoint(Point point)
        {
            return point.CoordX >= 0 && point.CoordY >= 0 &&
                   point.CoordX < Cells.Count &&
                   point.CoordY < Cells.Count;
        }

        public bool EmptyCell(Point point) => this[point].IsEmpty;

        public ChessPiece GetCellPiece(Point point) => this[point].Piece;

        public Point ConvertCoords(int coordY, char coordX)
        {
            coordX = Char.ToLower(coordX);
            return new Point(coordX - 'a', Cells.Count - coordY);
        }

        public List<Way> LegalWays(Color playerColor, Side playerSide)
        {
            List<Way> result = new List<Way>();
            List<Way> ways = FindAllWays(playerColor, playerSide);
            foreach (Way way in ways)
            {
                if (VerifyLegal(way, playerColor, playerSide))
                {
                    result.Add(way);
                }
            }
            return result;
        }
        public List<Way> LegalWaysFromPoint(Point startPoint, Color color, Side side)
        {
            List<Way> allPieceWays = FindPieceWays(startPoint, color, side, false);
            List<Way> legalWays = new List<Way>();
            foreach (Way way in allPieceWays)
            {
                if (VerifyLegal(way, color, side))
                {
                    legalWays.Add(way);
                }
            }
            return legalWays;
        }
        public List<Way> LegalWaysToPoint(Point endPoint, Color color, Side side)
        {
            List<Way> result = new List<Way>();
            List<Way> legalWays = LegalWays(color, side);
            foreach (Way way in legalWays)
            {
                if (way.End() == endPoint)
                {
                    result.Add(way);
                }
            }
            return result;
        }
        public bool VerifyLegal(Way way, Color playerColor, Side playerSide)
        {
            bool inCheck = false;
            if (way.SpecialType == SpecialWayType.Castling)
            {
                Direction dir = way.Direction;
                Point curPoint = new Point(way.Start());
                int limit = Math.Abs(way.End().CoordX - way.Start().CoordX);
                for (int i = 0; i <= limit; i++)
                {
                    if (!IsSaftyPoint(curPoint, playerColor, playerSide))
                    {
                        return false;
                    }
                    curPoint.MovePoint(dir);
                }
            }
            else
            {
                MakeMove(way);
                inCheck = KingInCheck(playerColor, playerSide);
                ReverseMove(way);
            }
            return !inCheck;
        }
        public void ClearWayPoints()
        {
            foreach (List<Cell> cells in Cells)
            {
                foreach (Cell cell in cells)
                {
                    cell.ClearWayPoints();
                }
            }
        }
        public void Clear()
        {
            foreach (List<Cell> cells in Cells)
            {
                foreach (Cell cell in cells)
                {
                    cell.Clear();
                }
            }
        }
        public void ReverseMove(Way way)
        {
            if (way.ChessPiece.Type == PieceType.King) ChangeKingPoint(way.ChessPiece.Color, way.Start());
            way.ChessPiece.MoveNumber -= 2;
            MakeMove(new Way(GetCellPiece(way.End()), way.End(), way.Start(), way.Direction));
            if (way.SpecialType == SpecialWayType.Castling)
            {
                Direction dir = way.Direction;
                Point newPlace = CastleRooKPoint(dir, way.End().CoordY);
                Point prevPoint = new Point(way.End(), Program.OppositeDirection(dir));
                MakeMove(new Way(GetCellPiece(prevPoint), prevPoint, newPlace, dir));
                way.ChessPiece.MoveNumber = 0;
                GetCellPiece(prevPoint).MoveNumber = 0;
            }
            if (way.AttackWay)
            {
                Point enPoint = new Point(way.End());
                if (way.SpecialType == SpecialWayType.Enpassant)
                {
                    Direction dir;
                    if (way.Direction == Direction.LeftDown ||
                        way.Direction == Direction.RightDown)
                    {
                        dir = Direction.Up;
                    }
                    else
                    {
                        dir = Direction.Down;
                    }
                    enPoint = new Point(way.End(), dir);
                }
                AddPiecePoint(way.End(), way.EnPieceColor());
                this[enPoint].IsEmpty = false;
                this[enPoint].Piece = way.EnChessPiece;
            }
        }
        private void ChangeKingPoint(Color kingColor, Point newPoint)
        {
            if (kingColor == Color.White) WhiteKingPoint = newPoint;
            else BlackKingPoint = newPoint;
        }
        public void MakeMove(Way way)
        {
            Color myColor = way.ChessPiece.Color;
            Color enColor = way.EnPieceColor();
            if (way.ChessPiece.Type == PieceType.King) ChangeKingPoint(way.ChessPiece.Color, way.End());
            way.ChessPiece.MoveNumber++;
            if (way.SpecialType == SpecialWayType.Castling)
            {
                Point newKingPoint = new Point(way.End());
                Point prevPoint = CastleRooKPoint(way.Direction, newKingPoint.CoordY);
                Direction oppositeDir = Program.OppositeDirection(way.Direction);
                Point newPoint = new Point(newKingPoint, oppositeDir);
                MakeMove(new Way(GetCellPiece(newKingPoint), way.Start(), way.End(), way.Direction));
                MakeMove(new Way(GetCellPiece(prevPoint), prevPoint, newPoint, oppositeDir));
                return;
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
                this[new Point(way.End(), dir)].IsEmpty = true;
            }
            RemovePiecePoint(way.Start(), myColor);
            AddPiecePoint(way.End(), myColor);
            if (way.AttackWay) RemovePiecePoint(way.End(), enColor);
            this[way.End()].IsEmpty = false;
            this[way.End()].Piece = GetCellPiece(way.Start());
            this[way.Start()].IsEmpty = true;
        }

        public Point CastleRooKPoint(Direction dir, int horizontal)
        {
            if (dir == Direction.Left) return new Point(0, horizontal);
            else return new Point(Cells.Count - 1, horizontal);
        }

        public bool IsSaftyPoint(Point point, Color color, Side side)
        {
            List<Way> ways = new List<Way>();
            ways.AddRange(DirectedWays(color, point, AllDirections(), false));
            //ways.AddRange(FindKnightWays(point, color, false));
            Color enColor = Program.SwitchCol(color);
            Side enSide = Program.SwitchSide(side);
            Point curPoint;
            PieceType curType;
            foreach (Way way in ways)
            {
                curPoint = way.End();
                curType = way.EnChessType();
                if (!EmptyCell(curPoint) && GetCellPiece(curPoint).Color != color)
                {
                    if (curType == PieceType.Pawn)
                    {
                        List<Direction> enDirs = ChessPiece.PawnAttackDirations(enSide);
                        foreach (Direction dir in enDirs)
                        {
                            if (new Point(curPoint, dir) == point) return false;
                        }
                    }
                    else if (curType == PieceType.Bishop || curType == PieceType.Queen ||
                            (curType == PieceType.King &&
                            DistBetPoints(curPoint, point) < 1.5f) ||
                            curType == PieceType.Rook)
                    {
                        List<Direction> dirs = new List<Direction>();
                        if (curType != PieceType.Bishop) dirs.AddRange(ChessPiece.RookDiretions());
                        if (curType != PieceType.Rook) dirs.AddRange(ChessPiece.BishopDiretion());
                        if (dirs.Any(x => x == way.Direction)) return false;
                    }

                    //List<Way> enWays = FindPieceWays(curPoint, enColor, enSide, false);
                    //if (enWays.Any(x => x.End() == point)) return false;
                }
            }
            ways = FindKnightWays(point, color, false);
            return !ways.Any(x => x.AttackWay && x.EnChessPiece.Type == PieceType.Knight);
        }

        public List<Way> FindAllWays(Color playerColor, Side playerSide)
        {
            List<Way> result = new List<Way>();
            List<Point> points = GetPiecesPoints(playerColor);
            foreach (Point point in points)
            {
                result.AddRange(FindPieceWays(point, playerColor, playerSide, false));
            }
            return result;
        }

        public List<Point> GetPiecesPoints(Color playerColor)
        {
            return playerColor == Color.White ? WhitePieces : BlackPieces;
        }

        public int GetPiecesProfit(Color color)
        {
            int result = 0;
            List<Point> points = color == Color.White ? WhitePieces : BlackPieces;
            foreach(Point point in points)
            {
                result += GetCellPiece(point).Value();
            }
            return result;
        }

        public List<Way> FindPieceWays(Point point, Color color, Side side, bool guard)
        {
             if (EmptyCell(point)) return new List<Way>();
            PieceWays figureWays;
            switch (GetCellPiece(point).Type)
            {
                case PieceType.Pawn: return FindPawnWays(point, color, side, guard);
                case PieceType.Bishop:
                    {
                        figureWays = FindBishopWays;
                    }
                    break;
                case PieceType.Rook:
                    {
                        figureWays = FindRookWays;
                    }
                    break;
                case PieceType.Queen:
                    {
                        figureWays = FindQueenWays;
                    }
                    break;
                case PieceType.Knight:
                    {
                        figureWays = FindKnightWays;
                    }
                    break;
                case PieceType.King: return FindKingWays(point, color, side, guard);
                default:
                    {
                        figureWays = null;
                    }
                    break;
            }
            return figureWays?.Invoke(point, color, guard);
        }
        public int CountPieces()
        {
            //int count = 0;
            //foreach (List<Cell> cells in Cells)
            //{
            //    foreach (Cell cell in cells)
            //    {
            //        if (!cell.IsEmpty) count++;
            //    }
            //}
            return WhitePieces.Count + BlackPieces.Count;
        }
        public static double DistBetPoints(Point point1, Point point2) =>
             Math.Sqrt(Math.Pow(point1.CoordX - point2.CoordX, 2) +
                 Math.Pow(point1.CoordY - point2.CoordY, 2));
        public static double DistToCentre(Point point)
        {
            List<double> distances = new List<double>
            {
                DistBetPoints(point, new Point(4, 4)),
                DistBetPoints(point, new Point(3, 3)),
                DistBetPoints(point, new Point(4, 3)),
                DistBetPoints(point, new Point(3, 4))
            };
            return distances.Min();
        }
        public void TestFill()
        {
            //foreach (List<Cell> cells in Cells)
            //{
            //    foreach (Cell cell in cells)
            //    {
            //        if (cell.Piece.Type != PieceType.King)
            //            cell.IsEmpty = true;
            //    }
            //}

            //this[new Point(6, 1)].Piece = new ChessPiece(Color.Black, PieceType.King);
            //this[new Point(6, 1)].IsEmpty = false;
            //this[new Point(5, 2)].Piece = new ChessPiece(Color.Black, PieceType.Rook);
            //this[new Point(5, 2)].IsEmpty = false;
            //this[new Point(6, 2)].Piece = new ChessPiece(Color.Black, PieceType.Pawn);
            //this[new Point(6, 2)].IsEmpty = false;
            //this[new Point(0, 3)].Piece = new ChessPiece(Color.Black, PieceType.Pawn);
            //this[new Point(0, 3)].IsEmpty = false;
            //this[new Point(7, 3)].Piece = new ChessPiece(Color.Black, PieceType.Pawn);
            //this[new Point(7, 3)].IsEmpty = false;
            //this[new Point(0, 4)].Piece = new ChessPiece(Color.White, PieceType.Pawn);
            //this[new Point(0, 4)].IsEmpty = false;
            //this[new Point(1, 4)].Piece = new ChessPiece(Color.Black, PieceType.Pawn);
            //this[new Point(1, 4)].IsEmpty = false;
            //this[new Point(2, 4)].Piece = new ChessPiece(Color.White, PieceType.Bishop);
            //this[new Point(2, 4)].IsEmpty = false;
            //this[new Point(5, 4)].Piece = new ChessPiece(Color.White, PieceType.Pawn);
            //this[new Point(5, 4)].IsEmpty = false;
            //this[new Point(4, 5)].Piece = new ChessPiece(Color.White, PieceType.Knight);
            //this[new Point(4, 5)].IsEmpty = false;
            //this[new Point(6, 5)].Piece = new ChessPiece(Color.White, PieceType.Pawn);
            //this[new Point(6, 5)].IsEmpty = false;
            //this[new Point(7, 5)].Piece = new ChessPiece(Color.White, PieceType.Rook);
            //this[new Point(7, 5)].IsEmpty = false;
            //this[new Point(1, 6)].Piece = new ChessPiece(Color.White, PieceType.Pawn);
            //this[new Point(1, 6)].IsEmpty = false;
            //this[new Point(3, 6)].Piece = new ChessPiece(Color.White, PieceType.Bishop);
            //this[new Point(3, 6)].IsEmpty = false;
            //this[new Point(4, 7)].Piece = new ChessPiece(Color.White, PieceType.King);
            //this[new Point(4, 7)].IsEmpty = false;

            //BlackKingPoint = new Point(6, 1);
            //WhiteKingPoint = new Point(4, 7);






            this[new Point(3, 0)].IsEmpty = true;
            this[new Point(5, 0)].IsEmpty = true;
            this[new Point(3, 1)].IsEmpty = true;
            this[new Point(4, 1)].IsEmpty = true;
            this[new Point(6, 1)].IsEmpty = true;
            this[new Point(1, 6)].IsEmpty = true;
            this[new Point(2, 6)].IsEmpty = true;
            this[new Point(3, 6)].IsEmpty = true;
            this[new Point(4, 6)].IsEmpty = true;
            this[new Point(6, 7)].IsEmpty = true;

            this[new Point(5, 2)].Piece = new ChessPiece(Color.Black, PieceType.Queen);
            this[new Point(5, 2)].IsEmpty = false;
            this[new Point(6, 2)].Piece = new ChessPiece(Color.Black, PieceType.Pawn);
            this[new Point(6, 2)].IsEmpty = false;
            this[new Point(3, 3)].Piece = new ChessPiece(Color.White, PieceType.Knight);
            this[new Point(3, 3)].IsEmpty = false;
            this[new Point(4, 3)].Piece = new ChessPiece(Color.Black, PieceType.Bishop);
            this[new Point(4, 3)].IsEmpty = false;
            this[new Point(4, 4)].Piece = new ChessPiece(Color.White, PieceType.Pawn);
            this[new Point(4, 4)].IsEmpty = false;
            this[new Point(1, 5)].Piece = new ChessPiece(Color.White, PieceType.Pawn);
            this[new Point(1, 5)].IsEmpty = false;
            this[new Point(2, 5)].Piece = new ChessPiece(Color.White, PieceType.Pawn);
            this[new Point(2, 5)].IsEmpty = false;
            this[new Point(3, 5)].Piece = new ChessPiece(Color.Black, PieceType.Pawn);
            this[new Point(3, 5)].IsEmpty = false;

        }
        public List<Way> FindKingWays(Point point, Color color, Side side, bool guard)
        {
            List<Way> ways = new List<Way>();
            List<Direction> dirs = AllDirections();
            bool emptyCell;
            Color pieceColor = new Color();
            foreach (Direction dir in dirs)
            {
                Point curPoint = new Point(point, dir);
                if (VerifyPoint(curPoint))
                {
                    emptyCell = EmptyCell(curPoint);
                    if (!emptyCell) pieceColor = GetCellPiece(curPoint).Color;
                    if ((guard && !emptyCell && pieceColor == color) ||
                        (!guard && (emptyCell || pieceColor != color)))
                    {
                        ways.Add(new Way(GetCellPiece(point), point, curPoint, !emptyCell, dir, GetCellPiece(curPoint)));
                        //ways.Last().AttackWay = !emptyCell; // && pieceColor != color
                        //if (!emptyCell) ways.Last().EnChessPiece = GetCellPiece(curPoint);
                    }
                }
            }
            if (GetCellPiece(point).FirstMove())
            {
                List<Point> rooksPoints;
                if (side == Side.Top)
                {
                    rooksPoints = new List<Point>
                    {
                        new Point(0, 0),
                        new Point(7, 0)
                    };
                }
                else
                {
                    rooksPoints = new List<Point>
                    {
                        new Point(0, 7),
                        new Point(7, 7)
                    };
                }
                dirs = new List<Direction>();
                foreach (Point item in rooksPoints)
                {
                    if (!EmptyCell(item) &&
                        GetCellPiece(item).Type == PieceType.Rook &&
                        GetCellPiece(item).FirstMove())
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
                    int limit = 2;
                    bool castling = true;
                    if (dir == Direction.Left) limit = 3;
                    for (int i = 0; i < limit; i++)
                    {
                        curPoint.MovePoint(dir);
                        if (!EmptyCell(curPoint))
                        {
                            castling = false;
                            break;
                        }
                    }
                    if (castling)
                    {
                        if (dir == Direction.Left) curPoint.MovePoint(Direction.Right);
                        ways.Add(new Way(GetCellPiece(point), point, curPoint, SpecialWayType.Castling, dir));
                    }
                }
            }
            return ways;
        }

        public List<Way> FindKnightWays(Point point, Color color, bool guard)
        {
            List<Way> ways = new List<Way>();
            Color pieceColor = new Color();
            bool emptyCell;
            List<List<Direction>> dirs = ChessPiece.KnightDiretion();
            foreach (List<Direction> directions in dirs)
            {
                Point curPoint = new Point(point);
                foreach (Direction dir in directions)
                {
                    curPoint.MovePoint(dir);
                }
                if (VerifyPoint(curPoint))
                {
                    emptyCell = EmptyCell(curPoint);
                    if (!emptyCell) pieceColor = GetCellPiece(curPoint).Color;
                    if ((guard && !emptyCell && pieceColor == color) ||
                        (!guard && (emptyCell || pieceColor != color)))
                    {
                        ways.Add(new Way(GetCellPiece(point), point, curPoint, !emptyCell, new Direction(), GetCellPiece(curPoint)));
                        //ways.Last().AttackWay = !emptyCell;// && pieceColor != color
                        //if (!emptyCell) ways.Last().EnChessPiece = GetCellPiece(curPoint);
                    }
                }
            }
            return ways;
        }

        public List<Way> FindQueenWays(Point point, Color color, bool guard) => DirectedWays(color, point, AllDirections(), guard);

        public List<Way> FindBishopWays(Point point, Color color, bool guard) => DirectedWays(color, point, ChessPiece.BishopDiretion(), guard);

        public List<Way> FindRookWays(Point point, Color color, bool guard) => DirectedWays(color, point, ChessPiece.RookDiretions(), guard);

        public List<Way> FindPawnWays(Point point, Color color, Side side, bool guard)
        {
            ChessPiece figure = GetCellPiece(point);
            Point curPoint = new Point(point);
            List<Way> resultWays = new List<Way>();
            Direction verticalDir = ChessPiece.PawnMoveDirections(side);
            List<Direction> diagDirs = ChessPiece.PawnAttackDirations(side);
            curPoint.MovePoint(verticalDir);
            if (!guard && VerifyPoint(curPoint) &&
                EmptyCell(curPoint))
            {
                resultWays.Add(new Way(figure, point, curPoint, verticalDir));
                curPoint.MovePoint(verticalDir);
                if (figure.FirstMove() &&
                    VerifyPoint(curPoint) &&
                    EmptyCell(curPoint))
                {
                    resultWays.Add(new Way(figure, point, curPoint, verticalDir));
                }
            }
            foreach (Direction dir in diagDirs)
            {
                curPoint = new Point(point);
                curPoint.MovePoint(dir);
                if (VerifyPoint(curPoint))
                {
                    Point prevPoint = new Point(curPoint, verticalDir);
                    Point newPoint = new Point(curPoint, Program.OppositeDirection(verticalDir));
                    if (!EmptyCell(curPoint))
                    {
                        Color pieceColor = GetCellPiece(curPoint).Color;
                        if ((guard && pieceColor == color) ||
                            (!guard && pieceColor != color))
                            resultWays.Add(new Way(figure, point, curPoint, true, dir, GetCellPiece(curPoint)));
                    }
                    else if (VerifyPoint(prevPoint) &&
                            VerifyPoint(newPoint) &&
                            this[prevPoint].Track &&
                            this[newPoint].Track &&
                            !EmptyCell(newPoint) &&
                            GetCellPiece(newPoint).Type == PieceType.Pawn)
                    {
                        resultWays.Add(new Way(figure, point, curPoint, true, SpecialWayType.Enpassant, dir, GetCellPiece(newPoint)));
                    }
                }
            }
            return resultWays;
        }

        public List<Way> DirectedWays(Color playerColor, Point point, List<Direction> dirs, bool guard)
        {
            List<Way> ways = new List<Way>();
            Color pieceColor = new Color();
            foreach (Direction dir in dirs)
            {
                Point curPoint = new Point(point);
                bool endOfPass = false;
                while (!endOfPass)
                {
                    curPoint.MovePoint(dir);
                    if (VerifyPoint(curPoint))
                    {
                        bool emptyCell = EmptyCell(curPoint);
                        endOfPass = !emptyCell;
                        if (!emptyCell) pieceColor = GetCellPiece(curPoint).Color;
                        if ((guard && !emptyCell && pieceColor == playerColor) ||
                            (!guard && (emptyCell || pieceColor != playerColor)))
                        {
                            ways.Add(new Way(GetCellPiece(point), point, curPoint, !emptyCell, dir, GetCellPiece(curPoint)));
                            //ways.Last().AttackWay = !emptyCell;// && pieceColor != playerColor
                            //if (!emptyCell) ways.Last().EnChessPiece = GetCellPiece(curPoint);
                        }
                    }
                    else endOfPass = true;
                }
            }
            return ways;
        }
        public void PawnTransformation(Way way)
        {
            if (way.GetPieceType(this) == PieceType.Pawn &&
               (way.End().CoordY == 0 ||
                way.End().CoordY == Cells.Count - 1))
            {
                way.ChessPiece.Type = PieceType.Queen;
            }
        }
        public void MakeTurn(Way way)
        {
            MakeMove(way);
            this[way.End()].Track = true;
            this[way.Start()].Track = true;
        }
        public bool KingInCheck(Color playerColor, Side playerSide)
        {
            //List<Way> enemyWays = FindAllWays(Program.SwitchCol(playerColor), Program.SwitchSide(playerSide));
            //List<Way> enAttacks = enemyWays.Where(x => x.AttackWay).ToList();
            //foreach (Way way in enemyWays)
            //{
            //    if (way.End() == point)
            //    {
            //        return true;
            //    }
            //}
            return !IsSaftyPoint(KingPoint(playerColor), playerColor, playerSide);
        }
        public Point KingPoint(Color playerColor)
        {
            //List<Point> points = ChessPiecesPoints(playerColor, PieceType.King);
            //return points.Count == 0 ? new Point() : points.First();
            if (playerColor == Color.White) return WhiteKingPoint;
            else return BlackKingPoint;
        }
        public bool PawnTransformation(Side playerSide, Way way)
        {
            int lastLine;
            if (playerSide == Side.Bottom) lastLine = 0;
            else lastLine = Cells.Count - 1;
            return WayPieceType(way) == Chess.PieceType.Pawn &&
                   way.End().CoordY == lastLine;
        }
        public bool Checkmate(Color playerColor, Side playerSide)
        {
            return LegalWays(playerColor, playerSide).Count == 0 &&
                   KingInCheck(playerColor, playerSide);
        }
        public bool Checkmate(Color playerColor, Side playerSide, List<Way> legalWays)
        {
            return legalWays.Count == 0 &&
                   KingInCheck(playerColor, playerSide);
        }
        public bool Stalemate(Color playerColor, Side playerSide)
        {
            return LegalWays(playerColor, playerSide).Count == 0 &&
                   !KingInCheck(playerColor, playerSide);
        }
        public PieceType WayPieceType(Way way)
        {
            return GetCellPiece(way.Start()).Type;
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
