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
        public Point()
        {
            CoordX = 0;
            CoordY = 0;
        }
        public Point(Point point, Direction dir)
        {
            CoordX = point.CoordX;
            CoordY = point.CoordY;
            MovePoint(dir);
        }
        public void MovePoint(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    {
                        CoordY--;
                    }
                    break;
                case Direction.Down:
                    {
                        CoordY++;
                    }
                    break;
                case Direction.Right:
                    {
                        CoordX++;
                    }
                    break;
                case Direction.Left:
                    {
                        CoordX--;
                    }
                    break;
                case Direction.RightUp:
                    {
                        CoordX++;
                        CoordY--;
                    }
                    break;
                case Direction.LeftUp:
                    {
                        CoordX--;
                        CoordY--;
                    }
                    break;
                case Direction.RightDown:
                    {
                        CoordX++;
                        CoordY++;
                    }
                    break;
                case Direction.LeftDown:
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
