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
        public void Fill(int length, int height, Color piecesColor)
        {
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
                        if (Cells[i][j].ChessPiece.Color == Color.White)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        PieceType type = Cells[i][j].ChessPiece.Type;
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

        public bool EmptyCell(Point point)
        {
            return this[point].IsEmpty;
        }

        public ChessPiece GetPieceOfCell(Point point)
        {
            return this[point].ChessPiece;
        }

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
            List<Way> allPieceWays = FindUnlegalPieceWays(startPoint, color, side);
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
            Color enemyColor = Program.SwitchCol(playerColor);
            Side enemySide = Program.SwitchSide(playerSide);
            List<Way> enWays = FindAllWays(enemyColor, enemySide);
            if (way.SpecialType == SpecialWayType.Castling)
            {
                Direction dir = way.Direction;
                Point curPoint = new Point(way.Start());
                int limit = Math.Abs(way.End().CoordX - way.Start().CoordX);
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
                MovePiece(way);
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
            way.ChessPiece.MoveNumber -= 2;
            MovePiece(new Way(GetPieceOfCell(way.End()), way.End(), way.Start(), way.Direction));
            if (way.SpecialType == SpecialWayType.Castling)
            {
                Direction dir = way.Direction;
                way.ChessPiece.MoveNumber--;
                Point newPlace = CastleRooKPoint(dir, way.End().CoordY);
                Point prevPoint = new Point(way.End(), Program.OppositeDirection(dir));
                GetPieceOfCell(prevPoint).MoveNumber -= 2;
                MovePiece(new Way(GetPieceOfCell(prevPoint), prevPoint, newPlace, dir));
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
                this[enPoint].IsEmpty = false;
                this[enPoint].ChessPiece = way.EnChessPiece;
            }
        }
        public void MovePiece(Way way)
        {
            way.ChessPiece.MoveNumber++;
            if (way.AttackWay) way.EnChessPiece = GetPieceOfCell(way.End());
            if (way.SpecialType == SpecialWayType.Castling)
            {
                Point newKingPoint = new Point(way.End());
                Point prevPoint = CastleRooKPoint(way.Direction, newKingPoint.CoordY);
                Direction oppositeDir = Program.OppositeDirection(way.Direction);
                Point newPoint = new Point(newKingPoint, oppositeDir);
                MovePiece(new Way(GetPieceOfCell(newKingPoint), way.Start(), way.End(), way.Direction));
                MovePiece(new Way(GetPieceOfCell(prevPoint), prevPoint, newPoint, oppositeDir));
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
            this[way.End()].IsEmpty = false;
            this[way.End()].ChessPiece = GetPieceOfCell(way.Start());
            this[way.Start()].IsEmpty = true;
        }

        public Point CastleRooKPoint(Direction dir, int horizontal)
        {
            if (dir == Direction.Left) return new Point(0, horizontal);
            else return new Point(Cells.Count - 1, horizontal);
        }

        public bool VerPlaceUnderAttack(Point point, List<Way> enWays)
        {
            foreach (Way way in enWays)
            {
                if (way.End() == point)
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
                result.AddRange(FindUnlegalPieceWays(point, playerColor, playerSide));
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
                       GetPieceOfCell(curPoint).Color == playerColor)
                    {
                        points.Add(curPoint);
                    }
                }
            }
            return points;
        }

        public List<Way> PieceWays(Point point, Color color, Side side)
        {
            if (EmptyCell(point))
            {
                return new List<Way>();
            }
            FigureWays figureWays;
            switch (GetPieceOfCell(point).Type)
            {
                case Chess.PieceType.Pawn: return FindPawnWays(point, color, side);
                case Chess.PieceType.Bishop:
                    {
                        figureWays = FindBishopWays;
                    }
                    break;
                case Chess.PieceType.Rook:
                    {
                        figureWays = FindRookWays;
                    }
                    break;
                case Chess.PieceType.Queen:
                    {
                        figureWays = FindQueenWays;
                    }
                    break;
                case Chess.PieceType.Knight:
                    {
                        figureWays = FindKnightWays;
                    }
                    break;
                case Chess.PieceType.King:
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
        public List<Way> FindUnlegalPieceWays(Point point, Color color, Side side)
        {
            List<Way> ways = PieceWays(point, color, side);
            List<Way> result = new List<Way>();
            foreach (Way way in ways)
            {
                Point endPoint = way.End();
                if (EmptyCell(endPoint) || GetPieceOfCell(endPoint).Color != color)
                {
                    result.Add(way);
                }
            }
            return result;
        }
        public List<Way> WaysOfProtection(Point point, Color color, Side side)
        {
            List<Way> protectionWays = new List<Way>();
            List<Way> pieceWays = PieceWays(point, color, side);
            foreach (Way way in pieceWays)
            {
                Point endPoint = way.End();
                if (!EmptyCell(endPoint) && GetPieceOfCell(endPoint).Color == color)
                {
                    way.AttackWay = true;
                    protectionWays.Add(way);
                }
            }
            return protectionWays;
        }
        public int CountMaterial(Color pieceColor)
        {
            int material = 0;
            foreach (List<Cell> cells in Cells)
            {
                foreach (Cell cell in cells)
                {
                    if (!cell.IsEmpty &&
                        cell.FigureColor() == pieceColor)
                    {
                        material += cell.ChessPiece.PieceValue();
                    }
                }
            }
            return material;
        }
        public void VerifyAttack(Way way, Color color)
        {
            if (!EmptyCell(way.End()) &&
                GetPieceOfCell(way.End()).Color != color)
            {
                way.AttackWay = true;
                way.EnChessPiece = GetPieceOfCell(way.End());
            }
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
            foreach (List<Cell> cells in Cells)
            {
                foreach (Cell cell in cells)
                {
                    cell.IsEmpty = true;
                }
            }

            this[new Point(3, 1)].ChessPiece = new ChessPiece(Color.White, PieceType.King);
            this[new Point(3, 1)].IsEmpty = false;
            this[new Point(6, 6)].ChessPiece = new ChessPiece(Color.Black, PieceType.King);
            this[new Point(6, 6)].IsEmpty = false;
            this[new Point(4, 2)].ChessPiece = new ChessPiece(Color.White, PieceType.Pawn);
            this[new Point(4, 2)].IsEmpty = false;
            this[new Point(7, 7)].ChessPiece = new ChessPiece(Color.Black, PieceType.Queen);
            this[new Point(7, 7)].IsEmpty = false;

            //this[new Point(5, 2)].ChessPiece = new ChessPiece(Color.White, PieceType.Pawn);
            //this[new Point(5, 2)].IsEmpty = false;
            //this[new Point(5, 2)].ChessPiece = new ChessPiece(Color.White, PieceType.Pawn);
            //this[new Point(5, 2)].IsEmpty = false;
        }
        public List<Way> FindKingWays(Point point, Color color)
        {
            List<Way> ways = new List<Way>();
            List<Direction> dirs = AllDirections();
            foreach (Direction dir in dirs)
            {
                Point curPoint = new Point(point, dir);
                if (VerifyPoint(curPoint))
                {
                    ways.Add(new Way(GetPieceOfCell(point), point, curPoint, dir));
                    VerifyAttack(ways.Last(), color);
                }
            }
            if (GetPieceOfCell(point).FirstMove())
            {
                List<Point> rooksPoints = ChessPiecesPoints(color, PieceType.Rook);
                dirs = new List<Direction>();
                foreach (Point item in rooksPoints)
                {
                    if (GetPieceOfCell(item).FirstMove())
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
                        ways.Add(new Way(GetPieceOfCell(point), point, curPoint, SpecialWayType.Castling, dir));
                    }
                }
            }
            return ways;
        }

        public List<Way> FindKnightWays(Point point, Color color)
        {
            List<Way> ways = new List<Way>();
            List<List<Direction>> dirs = ChessPiece.KnightDiration();
            foreach (List<Direction> directions in dirs)
            {
                Point curPoint = new Point(point);
                foreach (Direction dir in directions)
                {
                    curPoint.MovePoint(dir);
                }
                if (VerifyPoint(curPoint))
                {
                    ways.Add(new Way(GetPieceOfCell(point), point, curPoint, directions.Last()));
                    VerifyAttack(ways.Last(), color);
                }
            }
            return ways;
        }

        public List<Way> FindQueenWays(Point point, Color color) => DirectedWays(color, point, AllDirections());

        public List<Way> FindBishopWays(Point point, Color color) => DirectedWays(color, point, ChessPiece.BishopDiration());

        public List<Way> FindRookWays(Point point, Color color) => DirectedWays(color, point, ChessPiece.RookDirations());

        public List<Way> FindPawnWays(Point point, Color color, Side side)
        {
            ChessPiece figure = GetPieceOfCell(point);
            Point curPoint = new Point(point);
            List<Way> resultWays = new List<Way>();
            Direction verticalDir = ChessPiece.PawnMoveDir(side);
            List<Direction> diagDirs = ChessPiece.PawnAttackDirations(side);
            curPoint.MovePoint(verticalDir);
            if (VerifyPoint(curPoint) &&
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
                        resultWays.Add(new Way(figure, point, curPoint, true, dir, GetPieceOfCell(curPoint)));
                    }
                    else if (VerifyPoint(prevPoint) &&
                            VerifyPoint(newPoint) &&
                            this[prevPoint].Track &&
                            this[newPoint].Track &&
                            !EmptyCell(newPoint) &&
                            GetPieceOfCell(newPoint).Type == Chess.PieceType.Pawn)
                    {
                        resultWays.Add(new Way(figure, point, curPoint, true, SpecialWayType.Enpassant, dir, GetPieceOfCell(newPoint)));
                    }
                }
            }
            return resultWays;
        }

        public List<Way> DirectedWays(Color playerColor, Point point, List<Direction> dirs)
        {
            List<Way> ways = new List<Way>();
            foreach (Direction dir in dirs)
            {
                Point curPoint = new Point(point);
                bool endOfPass = false;
                while (!endOfPass)
                {
                    curPoint.MovePoint(dir);
                    if (VerifyPoint(curPoint))
                    {
                        ways.Add(new Way(GetPieceOfCell(point), point, curPoint, dir));
                        VerifyAttack(ways.Last(), playerColor);
                        if (!EmptyCell(curPoint))
                        {
                            endOfPass = true;
                        }
                    }
                    else
                    {
                        endOfPass = true;
                    }
                }
            }
            return ways;
        }
        public void MakeTurn(Way way)
        {
            MovePiece(way);
            this[way.End()].Track = true;
            this[way.Start()].Track = true;
        }
        public bool KingInCheck(Color playerColor, Side playerSide)
        {
            Point point = KingPoint(playerColor);
            List<Way> enemyWays = FindAllWays(Program.SwitchCol(playerColor), Program.SwitchSide(playerSide));
            List<Way> enAttacks = enemyWays.Where(x => x.AttackWay).ToList();
            foreach (Way way in enemyWays)
            {
                if (way.End() == point)
                {
                    return true;
                }
            }
            return false;
        }
        public Point KingPoint(Color playerColor)
        {
            List<Point> points = ChessPiecesPoints(playerColor, PieceType.King);
            return points.Count == 0 ? new Point() : points.First();
        }
        public List<Point> ChessPiecesPoints(Color color, PieceType figureType)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < Cells.Count; i++)
            {
                for (int j = 0; j < Cells[i].Count; j++)
                {
                    Point curPoint = new Point(j, i);
                    ChessPiece curFigure = GetPieceOfCell(curPoint);
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
        public bool PawnTransform(Side playerSide, Way way)
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
        public bool Stalemate(Color playerColor, Side playerSide)
        {
            return LegalWays(playerColor, playerSide).Count == 0 &&
                   !KingInCheck(playerColor, playerSide);
        }
        public PieceType WayPieceType(Way way)
        {
            return GetPieceOfCell(way.Start()).Type;
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
