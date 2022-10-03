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
        public ChessPiece ChessPiece { get; set; }

        [JsonProperty("IsEmpty")]
        public bool IsEmpty { get; set; }

        [JsonProperty("Track")]
        public bool Track { get; set; }

        [JsonProperty("WayPoint")]
        public bool WayPoint { get; set; }

        [JsonProperty("ChosenPiece")]
        public bool ChosenPoint { get; set; }

        public Cell(Color color, ChessPiece figure, bool isEmpty, bool track)
        {
            Color = color;
            ChessPiece = figure;
            IsEmpty = isEmpty;
            Track = track;
            WayPoint = false;
            ChosenPoint = false;
        }        
        public Color FigureColor()
        {
            return ChessPiece.Color;
        }
    }
}
