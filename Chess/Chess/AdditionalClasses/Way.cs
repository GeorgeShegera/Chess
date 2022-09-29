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

        [JsonProperty("SpecialWay")]    
        public bool SpecialWay { get; set; }

        public Way(Figure figure, List<Point> wayPoints, bool attackWay, bool specialWay)
        {
            Figure = figure;
            WayPoints = wayPoints;
            AttackWay = attackWay;
            SpecialWay = specialWay;
        }
        public Way()
        {
            Figure = new Figure();
            WayPoints = new List<Point>();
            AttackWay = false;
            SpecialWay = false;
        }
        public Way(Figure figure, Point prevPoint, Point newPoint, bool attackWay)
        {
            Figure = figure;
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = attackWay;
            SpecialWay = false;
        }
        public Way(bool specialWay, Figure figure, Point prevPoint, Point newPoint)
        {
            Figure = figure;
            WayPoints = new List<Point>
            {
                new Point(prevPoint),
                new Point(newPoint)
            };
            AttackWay = false;
            SpecialWay = specialWay;
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
