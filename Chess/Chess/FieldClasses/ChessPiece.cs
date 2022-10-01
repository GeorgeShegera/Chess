using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    public class ChessPiece
    {
        [JsonProperty("Color")]
        public Color Color { get; set; }

        [JsonProperty("Type")]
        public ChessPieceType Type { get; set; }

        [JsonProperty("FirstMove")]
        public bool FirstMove { get; set; }

        public ChessPiece(Color color, ChessPieceType type, bool firstMove)
        {
            Color = color;
            Type = type;
            FirstMove = firstMove;
        }
        public ChessPiece(Color color, ChessPieceType type)
        {
            Color = color;
            Type = type;
            FirstMove = false;
        }
        public ChessPiece()
        {
            Color = new Color();
            Type = new ChessPieceType();
            FirstMove = true;
        }
    }
}
