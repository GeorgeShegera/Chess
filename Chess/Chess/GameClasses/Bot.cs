using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Bot
    {
        [JsonProperty("Color")]
        public Color Color { get; set; }

        [JsonProperty("Side")]
        public Side Side { get; set; }

        public Bot(Color color, Side side)
        {
            Color = color;
            Side = side;
        }

        public Bot()
        {
            Color = new Color();
            Side = new Side();
        }

        public void SwitchColor()
        {
            Color = Program.SwitchColor(Color);
        }
        public void SwitchSide()
        {
            Side = Program.SwitchSide(Side);
        }
        public static double PieceProfit(ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.Pawn: return 1;
                case ChessPieceType.Bishop: return 3;
                case ChessPieceType.Knight: return 3;
                case ChessPieceType.Rook: return 5;
                case ChessPieceType.Queen: return 9;
                default: return 0;
            }
        }
        public Way BotTurn(Field field)
        {
            double maxProfit;
            Color enColor = Program.SwitchColor(Color);
            Side enSide = Program.SwitchSide(Side);
            List<Way> legalWays = field.AllLegalWays(Color, Side);
            foreach (Way way in legalWays)
            {
                if (way.AttackWay)// To verify the exchange
                {
                    way.Profit = CountExchangeProfit(field, way, 0, way.End(), Color, Side);
                }                
                else if (!VerifyCellUnderAtt(field, way.End()))// To verify the potential attack
                {
                    field.MovePiece(way);
                    List<Way> newWays = field.RealPieceWays(way.End(), Color, Side);
                    foreach(Way newWay in newWays)
                    {
                        if (newWay.AttackWay)
                        {
                            newWay.Profit += CountExchangeProfit(field, newWay, 0, newWay.End(), Color, Side);
                        }
                    }
                    maxProfit = newWays.Max(x => x.Profit);
                    if (maxProfit > 0)
                    {
                        way.Profit += maxProfit;
                    }
                    field.ReverseMove(way);
                }
                field.MovePiece(way);
                List<Way> enAttaks = FindAttackWays(field, enColor, enSide);
                if (enAttaks.Count > 0)
                {
                    double maxEnProfit = enAttaks.Max(x => x.Profit);
                    if (maxEnProfit > 0) way.Profit -= maxEnProfit;
                }
                if (!field.KingInCheck(Color, Side))
                {
                    if (field.Checkmate(enColor, enSide))
                    {
                        field.ReverseMove(way);
                        return way;
                    }
                    else if (field.KingInCheck(enColor, enSide))
                    {
                        way.Profit++;
                    }
                }
                field.ReverseMove(way);
            }
            maxProfit = legalWays.Max(x => x.Profit);            
            List<Way> res = legalWays.Where(x => x.Profit == maxProfit).ToList();
            return res[new Random().Next(0, res.Count)];
        }
        public double CountExchangeProfit(Field field, Way curWay, double profit, Point point, Color curColor, Side curSide)
        {
            field.MovePiece(curWay);
            double curProfit = PieceProfit(curWay.EnChessType());
            if (curColor == Color)
            {
                profit += curProfit;
            }
            else
            {
                profit -= curProfit;
            }
            List<Way> newWays = field.LegalWays(point, curColor, curSide);
            if(newWays.Count != 0)
            {
                Way way = newWays.OrderBy(x => PieceProfit(x.GetWayPiece(field))).ToList().First();
                profit = CountExchangeProfit(field, way, profit, point, Program.SwitchColor(curColor), Program.SwitchSide(curSide));
            }
            field.ReverseMove(curWay);
            return profit;
        }
        public List<Way> FindAttackWays(Field field, Color color, Side side)
        {
            List<Way> legalWays = field.AllLegalWays(color, side);
            List<Way> attackWays = new List<Way>();
            foreach (Way way in legalWays)
            {
                if (way.AttackWay)
                {
                    way.Profit += CountExchangeProfit(field, way, 0, way.End(), color, side);
                    attackWays.Add(way);
                }
            }
            return attackWays;
        }
        public bool VerifyCellUnderAtt(Field field, Point point)
        {            
            return field.LegalWays(point, Program.SwitchColor(Color), Program.SwitchSide(Side)).Count != 0;
        }
    }
}
