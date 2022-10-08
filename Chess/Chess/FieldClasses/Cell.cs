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

        [JsonProperty("KingInCheck")]
        public bool KingInCheck { get; set; }

        public Cell(Color color, ChessPiece chessPiece, bool isEmpty, bool track)
        {
            Color = color;
            ChessPiece = chessPiece;
            IsEmpty = isEmpty;
            Track = track;
            WayPoint = false;
            ChosenPoint = false;
            KingInCheck = false;
        }        
        public Color FigureColor()
        {
            return ChessPiece.Color;
        }
        public void ClearWayPoints()
        {
            WayPoint = false;
            ChosenPoint = false;
            KingInCheck = false;
        }
        public void Clear()
        {
            ClearWayPoints();
            Track = false;
        }
    }
}
