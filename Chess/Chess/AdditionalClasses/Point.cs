using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public Point(Point point)
        {
            CoordX = point.CoordX;
            CoordY = point.CoordY;
        }
        public void MovePoint(Diraction dir)
        {
            switch (dir)
            {
                case Diraction.Up:
                    {
                        CoordY--;
                    }
                    break;
                case Diraction.Down:
                    {
                        CoordY++;
                    }
                    break;
                case Diraction.Right:
                    {
                        CoordX++;
                    }
                    break;
                case Diraction.Left:
                    {
                        CoordX--;
                    }
                    break;
                case Diraction.RightUp:
                    {
                        CoordX++;
                        CoordY--;
                    }
                    break;
                case Diraction.LeftUp:
                    {
                        CoordX--;
                        CoordY--;
                    }
                    break;
                case Diraction.RightDown:
                    {
                        CoordX++;
                        CoordY++;
                    }
                    break;
                case Diraction.LeftDown:
                    {
                        CoordX--;
                        CoordY++;
                    }
                    break;
            }
        }
        public static bool operator ==(Point point1, Point point2)
        {
            return point1.CoordX == point2.CoordX && point1.CoordY == point2.CoordY;
        }
        public static bool operator !=(Point point1, Point point2)
        {
            return point1.CoordX != point2.CoordX || point1.CoordY != point2.CoordY;
        }
    }
}
