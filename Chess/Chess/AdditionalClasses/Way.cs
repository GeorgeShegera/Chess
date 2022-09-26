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
        public Way(Figure figure, List<Point> wayPoints, bool attackWay)
        {
            Figure = figure;
            WayPoints = wayPoints;
            AttackWay = attackWay;
        }
        public Way()
        {
            Figure = new Figure();
            WayPoints = new List<Point>();
            AttackWay = false;
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
