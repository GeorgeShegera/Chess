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

        public Way(Figure figure, List<Point> wayPoints)
        {
            Figure = figure;
            WayPoints = wayPoints;
        }
    }
}
