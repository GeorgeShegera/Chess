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
        public TypeOfFigure Type { get; set; }

        public Figure(Color color, TypeOfFigure type)
        {
            Color = color;
            Type = type;
        }
        public Figure()
        {
            Color = new Color();
            Type = new TypeOfFigure();
        }
    }
}
