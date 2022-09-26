using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    public class Figure
    {
        [JsonProperty("Color")]
        public Color Color { get; set; }

        [JsonProperty("Type")]
        public FigureType Type { get; set; }

        [JsonProperty("FirstMove")]
        public bool FirstMove { get; set; }

        public Figure(Color color, FigureType type, bool firstMove)
        {
            Color = color;
            Type = type;
            FirstMove = firstMove;
        }
        public Figure()
        {
            Color = new Color();
            Type = new FigureType();
            FirstMove = true;
        }
    }
}
