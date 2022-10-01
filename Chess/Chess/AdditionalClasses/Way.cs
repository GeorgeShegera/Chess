using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Way
    {
        [JsonProperty("Figure")]
        public Figure Figure { get; set; }

        [JsonProperty("WayPoints")]
        public List<Point> WayPoints { get; set; }

        [JsonProperty("AttackWay")]
        public bool AttackWay { get; set; }

        [JsonProperty("SpecialType")]
        public SpecialWayType SpecialType { get; set; }

        [JsonProperty("Direction")]
        public Direction Direction { get; set; }

        public Way(Figure figure, Point prevPoint, Point newPoint, bool attackWay, SpecialWayType specialType, Direction direction)
        {
            Figure = figure;
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = attackWay;
            SpecialType = specialType;
            Direction = direction;
        }
        public Way()
        {
            Figure = new Figure();
            WayPoints = new List<Point>();
            AttackWay = false;
            SpecialType = new SpecialWayType();
        }
        public Way(Figure figure, Point prevPoint, Point newPoint, bool attackWay, Direction direction)
        {
            Figure = figure;
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = attackWay;
            Direction = direction;
            SpecialType = new SpecialWayType();
        }
        public Way(Figure figure, Point prevPoint, Point newPoint, Direction direction)
        {
            Figure = figure;
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = false;
            SpecialType = new SpecialWayType();
            Direction = direction;
        }
        public Point NewPlace()
        {
            return WayPoints.Last();
        }
        public Point PrevPlace()
        {
            return WayPoints.First();
        }
    }
}
