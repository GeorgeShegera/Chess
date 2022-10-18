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
        public static List<Direction> PawnAttackDirations(Side side)
        {            
            if (side == Side.Bottom) return new List<Direction>
                                            {
                                                Direction.RightUp,
                                                Direction.LeftUp
                                            };
            else return new List<Direction>
                        {
                            Direction.RightDown,
                            Direction.LeftDown
                        };            
        }
        public static Direction PawnMoveDir(Side side)
        {            
            if(side == Side.Bottom) return Direction.Up;
            else return Direction.Down;
        }
        public static List<Direction> RookDirations() => new List<Direction>
                                                        {
                                                            Direction.Up,
                                                           Direction.Down,
                                                            Direction.Left,
                                                            Direction.Right
                                                        };
        public static List<Direction> BishopDiration() => new List<Direction>
                                                        {
                                                            Direction.LeftUp,
                                                            Direction.LeftDown,
                                                            Direction.RightUp,
                                                            Direction.RightDown
                                                        };
        public static List<List<Direction>> KnightDiration() => new List<List<Direction>>
                                                                {
                                                                    new List<Direction>
                                                                    {
                                                                        Direction.Left,
                                                                        Direction.LeftDown
                                                                    },
                                                                    new List<Direction>
                                                                    {
                                                                        Direction.Left,
                                                                        Direction.LeftUp
                                                                    },
                                                                    new List<Direction>
                                                                    {
                                                                        Direction.Right,
                                                                        Direction.RightDown
                                                                    },
                                                                    new List<Direction>
                                                                    {
                                                                        Direction.Right,
                                                                        Direction.RightUp
                                                                    },
                                                                    new List<Direction>
                                                                    {
                                                                        Direction.Up,
                                                                        Direction.RightUp
                                                                    },
                                                                    new List<Direction>
                                                                    {
                                                                        Direction.Up,
                                                                        Direction.LeftUp
                                                                    },
                                                                    new List<Direction>
                                                                    {
                                                                        Direction.Down,
                                                                        Direction.LeftDown
                                                                    },
                                                                    new List<Direction>
                                                                    {
                                                                        Direction.Down,
                                                                        Direction.RightDown
                                                                    }
                                                                };
        public int PieceProfit()
        {
            switch (Type)
            {
                case ChessPieceType.Pawn: return 1;
                case ChessPieceType.Bishop: return 3;
                case ChessPieceType.Knight: return 3;
                case ChessPieceType.Rook: return 5;
                case ChessPieceType.Queen: return 9;
                default: return 0;
            }
        }

    }
}
