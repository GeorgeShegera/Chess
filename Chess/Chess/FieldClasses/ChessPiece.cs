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
        public PieceType Type { get; set; }

        [JsonProperty("MoveNumber")]
        public int MoveNumber { get; set; }

        public ChessPiece(Color color, PieceType type, int firstMove)
        {
            Color = color;
            Type = type;
            MoveNumber = firstMove;
        }
        public ChessPiece(Color color, PieceType type)
        {
            Color = color;
            Type = type;
            MoveNumber = 1;
        }
        public ChessPiece()
        {
            Color = new Color();
            Type = new PieceType();
            MoveNumber = 0;
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
        public static Direction PawnMoveDirections(Side side)
        {            
            if(side == Side.Bottom) return Direction.Up;
            else return Direction.Down;
        }
        public static List<Direction> RookDiretions() => new List<Direction>
                                                        {
                                                            Direction.Up,
                                                            Direction.Down,
                                                            Direction.Left,
                                                            Direction.Right
                                                        };
        public static List<Direction> BishopDiretion() => new List<Direction>
                                                        {
                                                            Direction.LeftUp,
                                                            Direction.LeftDown,
                                                            Direction.RightUp,
                                                            Direction.RightDown
                                                        };
        public static List<List<Direction>> KnightDiretion() => new List<List<Direction>>
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
        public int PieceValue()
        {
            switch (Type)
            {
                case PieceType.Pawn: return 1;
                case PieceType.Bishop: return 3;
                case PieceType.Knight: return 3;
                case PieceType.Rook: return 5;
                case PieceType.Queen: return 9;
                default: return 0;
            }
        }
        public bool FirstMove() => MoveNumber == 0;
    }
}
