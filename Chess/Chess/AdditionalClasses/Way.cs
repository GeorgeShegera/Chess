using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Way
    {
        [JsonProperty("Figure")]
        public ChessPiece ChessPiece { get; set; }

        [JsonProperty("WayPoints")]
        public List<Point> WayPoints { get; set; }

        [JsonProperty("AttackWay")]
        public bool AttackWay { get; set; }

        [JsonProperty("EnChessPiece")]
        public ChessPiece EnChessPiece { get; set; }

        [JsonProperty("SpecialType")]
        public SpecialWayType SpecialType { get; set; }

        [JsonProperty("Direction")]
        public Direction Direction { get; set; }

        [JsonProperty("Profit")]
        public double Profit { get; set; } = 0;

        public Way(ChessPiece chPiece, Point prevPoint, Point newPoint, bool attackWay, SpecialWayType specialType, Direction direction, ChessPiece enChPiece)
        {
            ChessPiece = chPiece;
            EnChessPiece = enChPiece;
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = attackWay;
            SpecialType = specialType;
            Direction = direction;
            ChessPiece = chPiece;
        }
        public Way(ChessPiece chessPiece, Point prevPoint, Point newPoint, SpecialWayType specialType, Direction direction)
        {
            ChessPiece = chessPiece;
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = false;
            EnChessPiece = new ChessPiece();
            SpecialType = specialType;
            Direction = direction;            
        }
        public Way(Point prevPoint, Point newPoint)
        {
            ChessPiece = new ChessPiece();
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = false;
            EnChessPiece = new ChessPiece();
            SpecialType = SpecialWayType.Ordinary;
            Direction = new Direction();
        }
        public Way()
        {
            ChessPiece = new ChessPiece();
            EnChessPiece = new ChessPiece();
            WayPoints = new List<Point>();
            AttackWay = false;
            SpecialType = SpecialWayType.Ordinary;            
        }
        public Way(ChessPiece chPiece, Point prevPoint, Point newPoint, bool attackWay, Direction direction, ChessPiece enChPiece)
        {
            ChessPiece = chPiece;
            EnChessPiece = enChPiece;
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = attackWay;
            Direction = direction;
            SpecialType = SpecialWayType.Ordinary;
        }
        public Way(ChessPiece chPiece, Point prevPoint, Point newPoint, Direction direction)
        {
            ChessPiece = chPiece;
            EnChessPiece = new ChessPiece();
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = false;
            SpecialType = SpecialWayType.Ordinary;
            Direction = direction;
        }
        public Point End() => WayPoints.Last();
        public Point Start() => WayPoints.First();
        public ChessPieceType EnChessType() => EnChessPiece.Type;
        public ChessPieceType GetPieceType(Field field) => field.PieceOfCell(Start()).Type;        
        public Color EnChessColor() => EnChessPiece.Color;        
        public static bool operator ==(Way wayFirst, Way waySecond)
        {
            return wayFirst.End() == waySecond.End() &&
                   wayFirst.Start() == waySecond.Start();
        }
        public static bool operator !=(Way wayFirst, Way waySecond)
        {
            return wayFirst.End() != waySecond.End() ||
                   wayFirst.Start() != waySecond.Start();
        }
    }
}
