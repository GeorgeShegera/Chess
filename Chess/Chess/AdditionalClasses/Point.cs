using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Point
    {
        [JsonProperty("CoordX")]
        public int CoordX { get; set; }

        [JsonProperty("CoordY")]
        public int CoordY { get; set; }

        public Point(int coordX, int coordY)
        {
            CoordX = coordX;
            CoordY = coordY;
        }
    }
}
