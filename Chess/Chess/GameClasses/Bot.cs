using System;
using System.Collections.Generic;
using System.IO;
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
            Color = Program.SwitchCol(Color);
        }
        public void SwitchSide()
        {
            Side = Program.SwitchSide(Side);
        }

        public Way BotTurn(Field field)
        {
            double maxProfit;
            Color enColor = Program.SwitchCol(Color);
            Side enSide = Program.SwitchSide(Side);
            List<Way> legalWays = field.AllLegalWays(Color, Side);
            foreach (Way way in legalWays)
            {
                if (way.AttackWay)// To verify an exchange
                {
                    way.Profit = CountExchangeProfit(field, way, 0, way.End(), Color, Side);
                }                
                way.Profit -= GuardWaysProfit(field, way.Start());
                field.MovePiece(way);
                way.Profit += GuardWaysProfit(field, way.End());                
                if (!CellUnderAttack(field, way.End()))// To verify the potential attack
                {                   
                    List<Way> newWays = field.RealPieceWays(way.End(), Color, Side);
                    foreach (Way newWay in newWays)
                    {
                        if (newWay.AttackWay)
                        {
                            newWay.Profit += CountExchangeProfit(field, newWay, 0, newWay.End(), Color, Side);
                        }
                    }
                    if (newWays.Count != 0)
                    {
                        maxProfit = newWays.Max(x => x.Profit);
                        if (maxProfit > 0)
                        {
                            way.Profit += maxProfit / 2;
                        }
                    }
                }
                List<Way> enAttaks = FindAttackWays(field, enColor, enSide);
                if (enAttaks.Count > 0)
                {
                    double maxEnProfit = enAttaks.Max(x => x.Profit);
                    if (maxEnProfit < 0) way.Profit += maxEnProfit;
                }
                if (!field.KingInCheck(Color, Side))
                {
                    if (field.Checkmate(enColor, enSide))
                    {
                        field.ReverseMove(way);
                        return way;
                    }
                    //else if (field.KingInCheck(enColor, enSide))
                    //{
                    //    way.Profit++;
                    //}
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
            double curProfit = curWay.EnChessPiece.PieceProfit();
            if (curColor == Color) profit += curProfit;
            else profit -= curProfit;
            curColor = Program.SwitchCol(curColor);
            curSide = Program.SwitchSide(curSide);
            List<Way> newWays = field.LegalWays(point, curColor, curSide);
            if (newWays.Count != 0)
            {
                Way way = newWays.OrderBy(x => x.ChessPiece.PieceProfit()).ToList().First();
                profit = CountExchangeProfit(field, way, profit, point, curColor, curSide);
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
        public bool CellUnderAttack(Field field, Point point)
        {
            return field.LegalWays(point, Program.SwitchCol(Color), Program.SwitchSide(Side)).Count != 0;
        }
        public double GuardWaysProfit(Field field, Point point)
        {
            double resProfit = 0;
            Color enColor = Program.SwitchCol(Color);
            Side enSide = Program.SwitchSide(Side);
            List<Way> enAttackWays = FindAttackWays(field, enColor, enSide);
            List<Way> guardWays = field.WaysOfProtection(point, Color, Side);
            foreach (Way attackWay in enAttackWays)
            {
                if (attackWay.Profit <= 0)
                {
                    foreach (Way guardWay in guardWays)
                    {
                        if (attackWay.End() == guardWay.End())
                        {
                            resProfit += -attackWay.Profit;
                        }
                    }
                }
            }
            return resProfit;
        }
    }
}
