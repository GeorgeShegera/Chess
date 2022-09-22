using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Chess
{
    [Serializable]
    public class Cell
    {
        [JsonProperty("Color")]
        public Color Color { get; set; }

        [JsonProperty("Figure")]
        public Figure Figure { get; set; }

        [JsonProperty("IsEmpty")]
        public bool IsEmpty { get; set; }

        [JsonProperty("Track")]
        public bool Track { get; set; }


        public Cell(Color color, Figure figure, bool isEmpty, bool track)
        {
            Color = color;
            Figure = figure;
            IsEmpty = isEmpty;
            Track = track;
        }
    }
}
